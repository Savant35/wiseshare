using FluentResults;
using Microsoft.Extensions.Configuration;
using Moq;
using Sodium;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.Common.Interfaces.Email;
using Wiseshare.Application.Services;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Application.Common.Email;

    public class AuthenticationServiceTests{
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IWalletService> _walletServiceMock = new();
        private readonly Mock<IPortfolioService> _portfolioServiceMock = new();
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
        private readonly Mock<IEmailVerificationService> _verificationServiceMock = new();
        private readonly Mock<IEmailSender> _emailSenderMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            // make IConfiguration["Api:BaseUrl"] return something
            _configMock
                .Setup(c => c["Api:BaseUrl"])
                .Returns("https://api.example.com");

            // make email verification create token without throwing
            _verificationServiceMock
                .Setup(v => v.CreateToken(It.IsAny<Guid>(), It.IsAny<TimeSpan>()))
                .Returns("dummy-token");

            // email sender can be left as noop
            _emailSenderMock
                .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _authenticationService = new AuthenticationService(
                _userServiceMock.Object,
                _walletServiceMock.Object,
                _portfolioServiceMock.Object,
                _jwtTokenGeneratorMock.Object,
                _verificationServiceMock.Object,
                _emailSenderMock.Object,
                _configMock.Object
            );
        }

        [Fact]
        public async Task Test_RegisterAsync_WhenEmailAlreadyExists_ShouldFail()
        {
            var email = "ali@example.com";
            var existing = User.Create("X", "Y", email, "111", "Password1!", "q?", "a");

            _userServiceMock
                .Setup(u => u.GetUserByEmailAsync(email))
                .ReturnsAsync(Result.Ok(existing));

            var result = await _authenticationService.RegisterAsync(
                "Ali", "Arthur", email, "222", "Password1!", "q?", "a");

            Assert.True(result.IsFailed);
            Assert.Equal("A user with this email already exist", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_LoginAsync_WhenCredentialsAreValid_ShouldSucceed()
        {
            // Arrange
            var email = "ali@gmail.com";
            var rawPassword = "Password1!";
            var hashed = PasswordHash.ArgonHashString(rawPassword);
            var question = "what is your name";
            var answer = "ali";

            var user = User.Create("Ali", "Arthur", email, "814-123-456", hashed, question, answer);
            user.ConfirmEmail();                          // <- mark the user as verified

            _userServiceMock
                .Setup(u => u.GetUserByEmailAsync(email))
                .ReturnsAsync(Result.Ok(user));

            _jwtTokenGeneratorMock
                .Setup(j => j.GenerateToken(user))
                .Returns("generate-token");

            // Act
            var result = await _authenticationService.LoginAsync(email, rawPassword);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("generate-token", result.Value.Token);
            Assert.Equal("Ali", result.Value.FirstName);
            Assert.Equal("Arthur", result.Value.LastName);
            _jwtTokenGeneratorMock.Verify(j => j.GenerateToken(user), Times.Once);
        }


        [Fact]
        public async Task Test_LoginAsync_WhenEmailIsInvalid_ShouldFail()
        {
            var email = "nouser@example.com";
            _userServiceMock
                .Setup(u => u.GetUserByEmailAsync(email))
                .ReturnsAsync(Result.Fail<User>("Not found"));

            var result = await _authenticationService.LoginAsync(email, "anything");

            Assert.True(result.IsFailed);
            Assert.Equal("Invalid email or password.", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_ResetPasswordAsync_WhenValidInput_ShouldSucceed()
        {
            var email = "ali@gmail.com";
            var question = "q?";
            var answer = "yes";
            var user = User.Create("Ali", "A", email, "444", "ignored", question, answer);

            _userServiceMock
                .Setup(u => u.GetUserByEmailAsync(email))
                .ReturnsAsync(Result.Ok(user));
            _userServiceMock
                .Setup(u => u.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(Result.Ok());
            _userServiceMock
                .Setup(u => u.SaveAsync())
                .ReturnsAsync(Result.Ok());

            var result = await _authenticationService.ResetPasswordAsync(email, answer, "NewPass1!");

            Assert.True(result.IsSuccess);
        }
    }
