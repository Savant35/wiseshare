using FluentResults;
using Moq;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace wiseshare.Tests.UserServiceTests;
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
        private readonly UserService _userServicee;

        public UserServiceTests()
        {
            _userServicee = new UserService(_userRepoMock.Object);
        }

        [Fact]
        public async Task Test_GetUserById_WhenUserIdIsValid()
        {
            var userId = UserId.CreateUnique();
            var user1 = User.Create("Ali", "Arthur", "ali@example.com", "814-123-456", "securePassword", "what is your name", "ali");
            _userRepoMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .Returns(Task.FromResult(Result.Ok(user1)));

            var result = await _userServicee.GetUserByIdAsync(userId);

            Assert.True(result.IsSuccess);
            Assert.Equal("Ali", result.Value.FirstName);
        }

        [Fact]
        public async Task Test_GetUserById_WhenUserIdIsInvalid()
        {
            var userId = UserId.CreateUnique();
            _userRepoMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .Returns(Task.FromResult(Result.Fail<User>("User not found")));

            var result = await _userServicee.GetUserByIdAsync(userId);

            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_GetUserByEmail_WhenUserEmailIsValid()
        {
            var email = "ali@gmail.com";
            var user2 = User.Create("Ali", "Arthur", email, "814-123-456", "securePassword", "what is your name", "ali");
            _userRepoMock
                .Setup(x => x.GetUserByEmailAsync(email))
                .Returns(Task.FromResult(Result.Ok(user2)));

            var result = await _userServicee.GetUserByEmailAsync(email);

            Assert.True(result.IsSuccess);
            Assert.Equal(email, result.Value.Email);
        }

        [Fact]
        public async Task Test_GetUserByEmail_WhenUserEmailIsInvalid()
        {
            var email = "ali@gmail.com";
            _userRepoMock
                .Setup(x => x.GetUserByEmailAsync(email))
                .Returns(Task.FromResult(Result.Fail<User>("User not found")));

            var result = await _userServicee.GetUserByEmailAsync(email);

            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_GetUserByPhone_WhenUserPhoneIsValid()
        {
            var phone = "814-123-456";
            var user3 = User.Create("Ali", "Arthur", "ali@example.com", phone, "securePassword", "what is your name", "ali");
            _userRepoMock
                .Setup(x => x.GetUserByPhoneAsync(phone))
                .Returns(Task.FromResult(Result.Ok(user3)));

            var result = await _userServicee.GetUserByPhoneAsync(phone);

            Assert.True(result.IsSuccess);
            Assert.Equal(phone, result.Value.Phone);
        }

        [Fact]
        public async Task Test_GetUserByPhone_WhenPhoneIsInvalid()
        {
            var phone = "123456789";
            _userRepoMock
                .Setup(x => x.GetUserByPhoneAsync(phone))
                .Returns(Task.FromResult(Result.Fail<User>("User not found")));

            var result = await _userServicee.GetUserByPhoneAsync(phone);

            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_Save_WhenSaveIsValid()
        {
            _userRepoMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.FromResult(Result.Ok()));

            var result = await _userServicee.SaveAsync();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Test_Save_WhenSaveIsInvalid()
        {
            _userRepoMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.FromResult(Result.Fail("User information not saved")));

            var result = await _userServicee.SaveAsync();

            Assert.False(result.IsSuccess);
            Assert.Equal("User information not saved", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_Delete_WhenDeleteIsValid()
        {
            var userId = UserId.CreateUnique();
            _userRepoMock
                .Setup(x => x.DeleteAsync(userId))
                .Returns(Task.FromResult(Result.Ok()));

            var result = await _userServicee.DeleteAsync(userId);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Test_Delete_WhenDeleteIsInvalid()
        {
            var userId = UserId.CreateUnique();
            _userRepoMock
                .Setup(x => x.DeleteAsync(userId))
                .Returns(Task.FromResult(Result.Fail("Userid is invalid")));

            var result = await _userServicee.DeleteAsync(userId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Userid is invalid", result.Errors.First().Message);
        }

        [Fact]
        public async Task Test_Insert_WhenInsertIsValid()
        {
            var user = User.Create("Ali", "Arthur", "ali@example.com", "814-123-456", "securePassword", "what is your name", "ali");
            _userRepoMock
                .Setup(x => x.InsertAsync(user))
                .Returns(Task.FromResult(Result.Ok()));

            var result = await _userServicee.InsertAsync(user);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Test_GetUsers_WhenUsersAreValid()
        {
            var users = new List<User>
            {
                User.Create("ali","arthur","ali@gmail.com","123456789","password4","what is your name","ali"),
                User.Create("ali2","arthur2","ali2@gmail.com","123456789","password4","what is your name","ali"),
                User.Create("ali","arthur","ali@gmail.com","123456789","password4","what is your name","ali")
            };
            _userRepoMock
                .Setup(x => x.GetUsersAsync())
                .Returns(Task.FromResult(Result.Ok<IEnumerable<User>>(users)));

            var result = await _userServicee.GetUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count());
        }

        [Fact]
        public async Task Test_GetUsers_WhenUsersAreInvalid()
        {
            _userRepoMock
                .Setup(x => x.GetUsersAsync())
                .Returns(Task.FromResult(Result.Ok<IEnumerable<User>>(new List<User>())));

            var result = await _userServicee.GetUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }
    }
