using FluentResults;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.services;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using WiseShare.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IWalletService _walletService;
    private readonly IPortfolioService _portfolioService;

    public AuthenticationService(IUserService userService, IWalletService walletService, IPortfolioService portfolioService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _walletService = walletService;
        _portfolioService = portfolioService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // Handles user registration
    public Result Register(string firstName, string lastName, string email, string phone, string password)
    {
        // Create the objects 
        var user = User.Create(firstName, lastName, email, phone, password);
        var insertResult = _userService.Insert(user);

         if (insertResult.IsFailed)
        {
            //if user result fails return error
            return insertResult;
        }
        var wallet = Wallet.Create((UserId)user.Id);
        var portfolio = Portfolio.Create((UserId)user.Id);


        // Attempt to save the user using UserService
        var insertResult2 = _walletService.Insert(wallet);
        var insertResult3 = _portfolioService.Insert(portfolio);



        // Return success if the user was inserted successfully
        /*testing
        Console.WriteLine("wallet Balance : " + wallet.Balance);
        Console.WriteLine("wallet id: " + wallet.Id);
        Console.WriteLine("user id: " + user.Id);
        Console.WriteLine("portfolioId" + portfolio.Id);
        */
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