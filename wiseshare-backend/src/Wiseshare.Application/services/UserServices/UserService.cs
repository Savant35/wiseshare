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
    }

    public Result<IEnumerable<User>> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    public Result<User> GetUserByEmail(string email)
    {
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
        return _userRepository.Update(user);
    }

    public Result Save()
    {
        return _userRepository.Save();
    }

    public Result Delete(UserId userId)
    {
        return _userRepository.Delete(userId);
    }



}
