using Microsoft.AspNetCore.Mvc;
using WiseShare.Api.DTO.Authentication;
using WiseShare.Application.Authentication;
using FluentResults;

namespace WiseShare.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var result = _authenticationService.Register(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);
            /*

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }
        */

        return Ok(new { Message = "Registration successful" });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var result = _authenticationService.Login(request.Email, request.Password);

        if (result.IsFailed)
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        /*
        return Ok(new
        {
            Token = "generated-token", // Example hardcoded token; replace with your logic
            Message = "Login successful"
        });
        */
        //test
        //var (id,token, firstName, lastName) = result.Value;
        var (token, firstName, lastName) = result.Value;
        return Ok(new AuthenticationResponse(
        //Id:id,
        Token: token,
        FirstName: firstName,
        LastName: lastName));
    }
}
