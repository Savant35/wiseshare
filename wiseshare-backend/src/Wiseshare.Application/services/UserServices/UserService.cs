using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.RegularExpressions;
using EntityFramework.Exceptions.Common;
using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.UserServices;


public class UserService : IUserService
{



    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result<User> GetUserById(UserId userId)
    {
        return _userRepository.GetUserById(userId);


        //return _users.FirstOrDefault(user => user.Id.Value == userId);//user represent each individual item in the _list => means do this which is comparing
        //how the function would look like without using lambda function
        /*foreach (var user in _users)
    {
        if (user.Id.Value == userId)
        {
            return user;
        }
    }
    return null; */
    }
    public Result<IEnumerable<User>> GetUsers()
    {
        return _userRepository.GetUsers();

    }
    public Result<User> GetUserByEmail(string email)
    {
        //find the first user with a matching email then stop searching
        //return _users.FirstOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        return _userRepository.GetUserByEmail(email);
    }

    public Result<User> GetUserByPhone(string phone)
    {
        //find the user with the first matching phone number
        //return _users.FirstOrDefault(user => user.Phone == phone);
        return _userRepository.GetUserByPhone(phone);
    }

    public Result Insert(User user)
    {
        //validate password strength
        string passwordPatter = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{12,}$";
        if (!Regex.IsMatch(user.Password, passwordPatter))
        {
            return Result.Fail("password does not meet strength requirements requirements: length 12, 1 or more uppercase and lowercase, 1 or more digits and special characters((@$!%*#?&))");
        }

        // Validate email format
        try
        {
            var addr = new MailAddress(user.Email);
            if (addr.Address != user.Email)
            {
                return Result.Fail("The provided email address is invalid.");
            }
        }
        catch
        {
            return Result.Fail("The provided email address is invalid.");
        }


        try
        {

            return _userRepository.Insert(user);
        }
        catch (UniqueConstraintException e)
        {
            var message = e.InnerException?.Message ?? e.Message;
            //Console.WriteLine(message);

            if (message.Contains("Email"))
            {
                return Result.Fail("A user with the same Email already exists.");
            }
            else if (message.Contains("Phone"))
            {
                return Result.Fail("A user With the same Phone number already Exists");
            }
            return Result.Fail("User Registration failed DB Error");
        }
    }
    public Result Update(User user)
    {
        // if (User is null) return null; // Check if userId exist if not return null 

        // // Retrieve the existing user object from the repository
        // var user = _userRepository.Read(userId.Value);
        // if (user is null) return null;

        // //checks if the user Provide information to update if not return the current value by using the getter
        // var updatedEmail = !string.IsNullOrWhiteSpace(email) ? email : user.Email;
        // var updatedPhone = !string.IsNullOrWhiteSpace(phone) ? phone : user.Phone;
        // var updatedPassword = !string.IsNullOrWhiteSpace(password) ? password : user.Password;

        // // Call the repository to update the user with the updated values
        // return _userRepository.Update(userId, updatedEmail, updatedPhone, updatedPassword);
        return _userRepository.Update(user);
    }

    public Result Save()
    {
        return _userRepository.Save();
    }
    public Result Delete(UserId userId)
    {
        //delete a user by passed in ID
        return _userRepository.Delete(userId);
    }



}
