using System.Text.RegularExpressions;
using FluentResults;
using Sodium;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Application.Authentication;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Application.Common.Email;
using Wiseshare.Application.Common.Interfaces.Email;
using Microsoft.Extensions.Configuration;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IWalletService _walletService;
    private readonly IPortfolioService _portfolioService;
    private readonly IEmailVerificationService _verificationService;
    private readonly IEmailSender _emailSender;
    private readonly string _apiBaseUrl;

    public AuthenticationService(IUserService userService, IWalletService walletService,
    IPortfolioService portfolioService, IJwtTokenGenerator jwtTokenGenerator, IEmailVerificationService verificationService,
            IEmailSender emailSender, IConfiguration config){
        _userService = userService;
        _walletService = walletService;
        _portfolioService = portfolioService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _verificationService = verificationService;
        _emailSender = emailSender;
        _apiBaseUrl = config["Api:BaseUrl"] ?? throw new ArgumentNullException(nameof(config), "'Api:BaseUrl' is missing from configuration.");
    }

    // Handles user registration
    public async Task<Result> RegisterAsync(string firstName, string lastName, string email, string phone, string password,
    string SecurityQuestion, string SecurityAnswer){

        var existingUser = await _userService.GetUserByEmailAsync(email);
        if (existingUser.IsSuccess) return Result.Fail("A user with this email already exist");

        //validate password strength
        string passwordPatter = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
        if (!Regex.IsMatch(password, passwordPatter)){
            return Result.Fail("password does not meet strength requirements requirements: length 8, 1 or more uppercase and lowercase, 1 or more digits and special characters((@$!%*#?&))");
        }

        //hash password using argon 2
        string hashedPassword = PasswordHash.ArgonHashString(password);

        //create the different entities
        var user = User.Create(firstName, lastName, email, phone, hashedPassword, SecurityQuestion, SecurityAnswer);
        var wallet = Wallet.Create((UserId)user.Id);
        var portfolio = Portfolio.Create((UserId)user.Id);

        //insert all entities to the db
        var insertUserResult = await _userService.InsertAsync(user);
        var insertWalletResult = await _walletService.InsertAsync(wallet);
        var insertPortfolioResult = await _portfolioService.InsertAsync(portfolio);


        if (insertUserResult.IsFailed || insertWalletResult.IsFailed || insertPortfolioResult.IsFailed){
            var errors = new List<string>();
            if (insertUserResult.IsFailed){
                errors.AddRange(insertUserResult.Errors.Select(e => e.Message));
            }

            if (insertWalletResult.IsFailed){
                errors.AddRange(insertWalletResult.Errors.Select(e => e.Message));
            }

            if (insertPortfolioResult.IsFailed){
                errors.AddRange(insertPortfolioResult.Errors.Select(e => e.Message));
            }
            return Result.Fail("Registration failed: " + string.Join("; ", errors));
        }

        var saveResult = await _userService.SaveAsync();
        if (saveResult.IsFailed) return Result.Fail("Failed to save user.");
        

        /*var token = _verificationService.CreateToken(user.Id.Value, TimeSpan.FromHours(24));
        var link = $"{_apiBaseUrl}/auth/verify-email?token={Uri.EscapeDataString(token)}";
        var body = $@"<p>Hi {firstName},</p>
              <p>Click to verify your WiseShare email (expires in 24h):</p>
              <a href=""{link}"">{link}</a>";
        await _emailSender.SendAsync(email, "Verify your WiseShare email", body);
        */
        return Result.Ok();

    }


    // Handles user login
    public async Task<Result<(string Token, string FirstName, string LastName,string Role)>> LoginAsync(string email, string password){

        var userResult = await _userService.GetUserByEmailAsync(email);

        if (userResult.IsFailed){
            return Result.Fail("Invalid email or password.");
        }

        var user = userResult.Value;

        //validate if the account is active
        if (!user.IsActive){
            if(user.FailedLoginAttempts >= 5){ 
                return Result.Fail("Your account has been deactivated due to multiple failed login attempts.");
            }
            return Result.Fail("your account has been deactiavted. Please contact our support team at 999-999-9999");
        }

        bool isValid = PasswordHash.ArgonHashStringVerify(user.Password, password);
        // Validate the password if it fails return failed login
        if (!isValid){
            user.RegisterFailedLogin();
            await _userService.UpdateAsync(user);
            await _userService.SaveAsync();
            return Result.Fail("Invalid email or password.");
        }
        if(!user.IsEmailVerified) return Result.Fail("Please verify your email before logging in");


        // Return token containing user details 
        user.ResetLoginAttempts();
        await _userService.UpdateAsync(user);
        await _userService.SaveAsync();

        // Generate a JWT token
        var token = _jwtTokenGenerator.GenerateToken(user);
        return Result.Ok((token, user.FirstName, user.LastName,user.Role));
    }

    public async Task<Result> ResetPasswordAsync(string email, string securityAnswer, string newPassword){

        var userResult = await _userService.GetUserByEmailAsync(email);
        if (userResult.IsFailed) return Result.Fail("User not found.");

        var user = userResult.Value;
        if (!user.SecurityAnswer.Equals(securityAnswer, StringComparison.OrdinalIgnoreCase)){
            return Result.Fail("Security answer is incorrect.");
        }

        string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
        if (!Regex.IsMatch(newPassword, passwordPattern)){
            return Result.Fail("New password does not meet strength requirements. It must be at least 8 characters long and include uppercase, lowercase, digits, and special characters.");
        }

        string hashedPassword = PasswordHash.ArgonHashString(newPassword);
        user.ResetPassword(hashedPassword);

        // Update the user via the UserService and persist changes.
        var updateResult = await _userService.UpdateAsync(user);
        if (updateResult.IsFailed){
            return Result.Fail("Failed to update the password.");
        }

        var saveResult = await _userService.SaveAsync();
        if (saveResult.IsFailed){
            return Result.Fail("Changes failed to save");
        }
        return Result.Ok();
    }
}