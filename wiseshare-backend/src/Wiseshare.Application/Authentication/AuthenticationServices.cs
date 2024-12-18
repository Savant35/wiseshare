using FluentResults;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using WiseShare.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // Handles user registration
    public Result Register(string firstName, string lastName, string email, string phone, string password)
    {
        // Check if a user with the same email or phone already exists
        /*if (_userRepository.GetUserByEmail(email).IsSuccess || _userRepository.GetUserByPhone(phone).IsSuccess)
        {
            return Result.Fail("User with the same email or phone already exists.");
        }
        */

        // Create a new user
        var user = User.Create(firstName, lastName, email, phone, password);

        // Save the user to the repository
        _userRepository.Insert(user);

        // Return success
        return Result.Ok();
    }

    // Handles user login
    public Result<(string Token, string FirstName, string LastName)> Login(string email, string password)
    {
        // Retrieve the user by email
        var userResult = _userRepository.GetUserByEmail(email);

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