using FluentResults;
using Moq;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

public class UserServiceTests
{
    //Mock<IUserRepository> is a fake version of IUserRepository. it mimics the behavior of a
    //real repository without connecting to a actual database
    private readonly Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
    private IUserService _userServicee; //hold an instance of the UserService being tested
    public UserServiceTests()//passes the mock object of IUserRepo into user service so that the service interacts with the fake repo
    {
        _userServicee = new UserService(_userRepoMock.Object);
    }
    [Fact]//marks the following method as a test method
    public void Test_GetUserById_WhenUserIdIsValid()
    {
        //Arrange
        var userId = UserId.CreateUnique();
        var user1 = User.Create("ali", "arthur", "user1@gmail.com", "89324823479", "password");
        _userRepoMock.Setup(x => x.GetUserById(userId))
        .Returns(user1);

        //Act
        var result = _userServicee.GetUserById(userId);
        //assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ali", result.Value.FirstName); //(expected , then type)
    }

    [Fact]
    public void Test_GetUserById_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        _userRepoMock.Setup(x => x.GetUserById(userId))
            .Returns(Result.Fail<User>("User not found"));//tells mock to return a failed Result<user>

        // Act
        var result = _userServicee.GetUserById(userId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors.First().Message);
    }
    [Fact]
    public void Test_GetUserByEmail_WhenUserEmailIsValid()
    {
        var userId = UserId.CreateUnique();
        var useremail = "ali@gmail.com";
        var user2 = User.Create("ali", "arthur", useremail, "8122987984", "password");
        _userRepoMock.Setup(x => x.GetUserByEmail(useremail))
        .Returns(user2);

        var result = _userServicee.GetUserByEmail(useremail);

        Assert.True(result.IsSuccess);
        Assert.Equal("ali@gmail.com", result.Value.Email);

    }

    [Fact]
    public void Test_GetUserByEmail_WhenUserEmailIsInValid()
    {
        var userId = UserId.CreateUnique();
        var userEmail = "ali@gmail.com";
        _userRepoMock.Setup(x => x.GetUserByEmail(userEmail))
        .Returns(Result.Fail<User>("User not found"));

        var result = _userServicee.GetUserByEmail(userEmail);
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors.First().Message);
    }


    [Fact]
    public void Test_GetUserByPhone_WhenUserPhoneIsValid()
    {
        //Arrange
        var userId = UserId.CreateUnique();
        var userPhone = "123456789";
        var user3 = User.Create("ali3", "Arthur4", "ali3@gmail.com", userPhone, "password3");
        _userRepoMock.Setup(x => x.GetUserByPhone(userPhone))
        .Returns(user3);

        //Act
        var result = _userServicee.GetUserByPhone(userPhone);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("123456789", result.Value.Phone);

    }

    [Fact]
    public void Test_GetUserByPhone_WhenPhoneIsInvalid()
    {
        var userId = UserId.CreateUnique();
        var userPhone = "123456789";
        _userRepoMock.Setup(x => x.GetUserByPhone(userPhone))
        .Returns(Result.Fail<User>("User not found"));

        var result = _userServicee.GetUserByPhone(userPhone);

        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors.First().Message);

    }

    [Fact]
    public void Test_Save_WhenSaveIsValid()
    {
        _userRepoMock.Setup(x => x.Save())
        .Returns(Result.Ok);

        var result = _userServicee.Save();
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailed);
    }

    [Fact]
    public void Test_Save_WhenSaveIsInValid()
    {
        _userRepoMock.Setup(x => x.Save())
        .Returns(Result.Fail("User information not saved"));

        var result = _userServicee.Save();
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailed);
        Assert.Equal("User information not saved", result.Errors.First().Message);
    }

    [Fact]
    public void Test_Delete_WhenDeleteIsValid()
    {
        // Given
        var userId = UserId.CreateUnique();
        _userRepoMock.Setup(x => x.Delete(userId))
        .Returns(Result.Ok());

        // When
        var result = _userServicee.Delete(userId);
        // Then

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Test_Delete_WhenDeleteIsInValid()
    {
        // Given
        var userId = UserId.CreateUnique();
        _userRepoMock.Setup(x => x.Delete(userId))
        .Returns(Result.Fail("Userid is invalid"));

        // When
        var result = _userServicee.Delete(userId);
        // Then

        Assert.False(result.IsSuccess);
        Assert.Equal("Userid is invalid", result.Errors.First().Message);
    }

    [Fact]
    public void Test_Insert_WhenInsertIsValid()
    {
        //Arrange
        var userId = UserId.CreateUnique();
        var user = User.Create("ali", "arthur", "ali@gmail.com", "123456789", "password");
        _userRepoMock.Setup(x => x.Insert(user))
        .Returns(Result.Ok);

        //act 
        var result = _userServicee.Insert(user);

        //Assert
        Assert.True(result.IsSuccess);
    }

    /*
        [Fact]
        *public void Test_Insert_WhenInsertIsInvalid()
        {
            User? user = null;
            _userRepoMock.Setup(x => x.Insert(user))
            .Returns(Result.Fail("User cannot be null"));

            var result = _userServicee.Insert(user);

            Assert.False(result.IsSuccess);
            Assert.Equal("User cannot be null",result.Errors.First().Message);
        }
        */

    [Fact]
    public void Test_GetUsers_WhenUsersAreValid()
    {
        var users = new List<User>{
            User.Create("ali","arthur","ali@gmail.com","123456789","password4"),
            User.Create("ali2","arthur2","ali2@gmail.com","123456789","password4"),
            User.Create("ali","arthur","ali@gmail.com","123456789","password4")
        };
        // defines how GetUser should work
        _userRepoMock.Setup(x => x.GetUsers())
        .Returns(users);

        //act
        var result = _userServicee.GetUsers();

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count());//checks the amount of users mathc those in the list

        // Check the first user in the list
        var firstUser = result.Value.First();// or result.Value.First()
        Assert.Equal("ali", firstUser.FirstName);
        Assert.Equal("arthur", firstUser.LastName);
        Assert.Equal("ali@gmail.com", firstUser.Email);

        // Check the second user in the list
        var secondUser = result.Value.ElementAt(1);
        Assert.Equal("ali2", secondUser.FirstName);
        Assert.Equal("arthur2", secondUser.LastName);
        Assert.Equal("ali2@gmail.com", secondUser.Email);

        // Check the second user in the list
        var third = result.Value.ElementAt(2);
        Assert.Equal("ali", third.FirstName);
        Assert.Equal("arthur", third.LastName);
        Assert.Equal("ali@gmail.com", third.Email);
    }

    [Fact]
    public void Test_GetUsers_WhenUsersAreInValid()
    {

        var users = new List<User> { };// empty list to simulate no users found
        _userRepoMock.Setup(x => x.GetUsers())
        .Returns(Result.Ok<IEnumerable<User>>(users));

        var result = _userServicee.GetUsers();

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(new List<User>(), result.Value);
        //Assert.Equal(new List<User>{User.Create("ali","dkfjl","dfd","dklf","dkfkd")}, result.Value);//make the test fail on purpose


    }

}
