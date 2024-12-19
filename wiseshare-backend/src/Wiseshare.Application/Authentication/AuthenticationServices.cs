using FluentResults;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using WiseShare.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // Handles user registration
    public Result Register(string firstName, string lastName, string email, string phone, string password)
    {
        // Create a new user
        var user = User.Create(firstName, lastName, email, phone, password);

        // Attempt to save the user using UserService
        var insertResult = _userService.Insert(user);

        // Check if the insertion failed
        if (insertResult.IsFailed)
        {
            // Propagate the failure
            return Result.Fail(insertResult.Errors);
        }

        // Return success if the user was inserted successfully
        return Result.Ok();
    }


    // Handles user login
    public Result<(string Token, string FirstName, string LastName)> Login(string email, string password)
    {
        // Retrieve the user by email
        var userResult = _userService.GetUserByEmail(email);

        if (userResult.IsFailed)
        {
            return Result.Fail<(string Token, string FirstName, string LastName)>("Invalid email or password.");
        }

        var user = userResult.Value;

        // Validate the password
        if (user.Password != password)
        {
            return Result.Fail<(string Token, string FirstName, string LastName)>("Invalid email or password.");
        }

        // Generate a JWT token
        var token = _jwtTokenGenerator.GenerateToken(user);

        // Return token and user details
        return Result.Ok((token, user.FirstName, user.LastName));
    }
}