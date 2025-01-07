using FluentResults;
using Moq;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.services;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using Wiseshare.Domain.UserAggregate;
using WiseShare.Application.Authentication;

public class AuthenticationServiceTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
   private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();

     private readonly Mock<IWalletService> _walletServiceMock = new Mock<IWalletService>();
    private readonly Mock<IPortfolioService> _portfolioServiceMock = new Mock<IPortfolioService>();
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _authenticationService = new AuthenticationService(_userServiceMock.Object, _walletServiceMock.Object, _portfolioServiceMock.Object, _jwtTokenGeneratorMock.Object);
    }
    [Fact]
    public void Test_Register_WhenValidInput_ShouldSucceed()
    {
        // Arrange
        var firstName = "Ali";
        var lastName = "Arthur";
        var email = "ali@example.com";
        var phone = "123-456-7890";
        var password = "securePassword";

        var newUser = User.Create(firstName, lastName, email, phone, password);

        // Mock the repository to simulate successful insertion
        _userServiceMock.Setup(x => x.Insert(It.IsAny<User>()))
         .Returns(Result.Ok());

        // Act
        var result = _authenticationService.Register(firstName, lastName, email, phone, password);

        // Assert
        Assert.True(result.IsSuccess);
        //Assert.Empty(result.Errors);
        _userServiceMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Once); // Ensure Insert was called once
    }

    [Fact]
    public void Test_Register_WhenEmailAlreadyExists_ShouldFail()
    {
        // Arrange
        var firstName = "Ali";
        var lastName = "Arthur";
        var email = "ali@example.com";
        var phone = "123-456-7890";
        var password = "securePassword";

        // Mock the repository to simulate a duplicate email
        _userServiceMock.Setup(x => x.Insert(It.IsAny<User>()))
                     .Returns(Result.Fail("User already exists."));

        // Act
        var result = _authenticationService.Register(firstName, lastName, email, phone, password);

        // Assert
        Assert.True(result.IsFailed);
        //Assert.Contains(result.Errors, e => e.Message == "User already exists.");
        _userServiceMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Once); // Ensure Insert was called once
    }


    [Fact]
    public void Test_Login_WhenCredentialsAreValid()
    {
        //Arrange
        var email = "ali@gmail.com";
        var password = "password";
        var user = User.Create("Ali", "Arthur", email, "814-123-456", password);
        _userServiceMock.Setup(x => x.GetUserByEmail(email))
        .Returns(Result.Ok(user));
        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
        .Returns("generate-token");

        //Act
        var result = _authenticationService.Login(email, password);//returns result object of true if credentials false otherwise
        Assert.True(result.IsSuccess);
        Assert.Equal("generate-token", result.Value.Token);
        Assert.Equal("Ali", result.Value.FirstName);
        Assert.Equal("Arthur", result.Value.LastName);
        _jwtTokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);//verifies that token was generated if credentials are valid
    }
    [Fact]
    public void Test_Login_WhenEmailIsInvalid()
    {
        var email = "ali@gmail.com";
        var password = "password";
        var user = User.Create("Ali", "Arthur", email, "814-123-4567", password);
        _userServiceMock.Setup(x => x.GetUserByEmail(email))
        .Returns(Result.Fail<User>("User not found"));

        var result = _authenticationService.Login(email, password);
        Assert.True(result.IsFailed); // Ensure the result is a failure
        Assert.Equal("Invalid email or password.", result.Errors.First().Message); // Check the error message

        _jwtTokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never); // Ensure token generation was not called
    }

    [Fact]
    public void Test_Login_WhenPasswordIsInvalid()
    {
        //Arrange
        var email = "ali@gmail.com";
        var password = "password";
        var user = User.Create("Ali", "Arthur", email, "814-121-2134", password);
        _userServiceMock.Setup(x => x.GetUserByEmail(email))
        .Returns(Result.Ok(user));


        //act
        var result = _authenticationService.Login(email, "password12");
        //Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Invalid email or password.", result.Errors.First().Message);
    }


}