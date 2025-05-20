using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Authentication;
using Wiseshare.Application.Authentication;
using Wiseshare.Application.Common.Email;
using Wiseshare.Application.Common.Interfaces.Email;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using WiseShare.Api.DTO.Authentication;

namespace Wiseshare.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;
    private readonly IEmailVerificationService _verificationService;
    private readonly IEmailSender _emailSender;
    private readonly string _apiBaseUrl;

    public AuthenticationController(IAuthenticationService authenticationService, IUserService userService,
            IEmailVerificationService verificationService, IEmailSender emailSender, IConfiguration config){

        _authenticationService = authenticationService;
        _userService = userService;
        _verificationService = verificationService;
        _emailSender = emailSender;
        _apiBaseUrl = config["Api:BaseUrl"] ?? throw new ArgumentNullException(nameof(config), "'Api:BaseUrl' is missing.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request){

        var result = await _authenticationService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password,
            request.SecurityQuestion,
            request.SecurityAnswer);

        if (result.IsFailed){
            return BadRequest(new{
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
        }

        return Ok(new { Message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request){

        var result = await _authenticationService.LoginAsync(request.Email, request.Password);
        if (result.IsFailed)
        {
            return BadRequest(new
            {
                Message = "Login failed",
                Errors = result.Errors.Select(e => e.Message)
            });
        }
        var (token, firstName, lastName, role) = result.Value;
        return Ok(new AuthenticationResponse(
            Token: token,
            FirstName: firstName,
            LastName: lastName,
            Role: role));
    }

    [HttpGet("security-question")]
    public async Task<IActionResult> GetSecurityQuestion([FromQuery] string email){

        var userResult = await _userService.GetUserByEmailAsync(email);

        if (userResult.IsFailed){
            return NotFound(new{
                Message = "User not found",
                Errors = userResult.Errors.Select(e => e.Message)
            });
        }
        var user = userResult.Value;
        return Ok(new { SecurityQuestion = user.SecurityQuestion });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request){

        var result = await _authenticationService.ResetPasswordAsync(request.Email, request.SecurityAnswer, request.NewPassword);
        if (result.IsFailed){
            return BadRequest(new{
                Message = "Password reset failed",
                Errors = result.Errors.Select(e => e.Message)
            });
        }
        return Ok(new { Message = "Password reset successfully." });
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token){

        if (!_verificationService.TryValidateToken(token, out var userGuid))
            return BadRequest(new { Message = "Invalid or expired token" });

        var userRes = await _userService.GetUserByIdAsync(UserId.Create(userGuid));

        if (userRes.IsFailed)
            return NotFound(new { Message = "User not found" });

        var user = userRes.Value;
        if (user.IsEmailVerified)
            return BadRequest(new { Message = "Email already verified" });

        user.ConfirmEmail();
        await _userService.UpdateAsync(user);
        await _userService.SaveAsync();
        return Ok(new { Message = "Email verified successfully" });
    }

    [HttpPost("sendemail-verification")]
    public async Task<IActionResult> ResendVerification([FromQuery] string email){
        var userRes = await _userService.GetUserByEmailAsync(email);
        if (userRes.IsFailed)
            return NotFound(new { Message = "User not found" });

        var user = userRes.Value;
        if (user.IsEmailVerified)
            return BadRequest(new { Message = "This email is already verified." });

        var token = _verificationService.CreateToken(user.Id.Value, TimeSpan.FromHours(24));
        var link = $"{_apiBaseUrl}/auth/verify-email?token={Uri.EscapeDataString(token)}";

        var body = $@"<p>Hi {user.FirstName},</p>
                  <p>This is a new verification link. It will expire in 24 hours.</p>
                  <a href=""{link}"">{link}</a>";

        await _emailSender.SendAsync(user.Email, "Verify your WiseShare email", body);

        return Ok(new { Message = "Verification email sent." });
    }
}
