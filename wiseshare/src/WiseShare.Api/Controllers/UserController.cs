using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Users;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.UserAggregate.ValueObjects;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("search/email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email){

        var result = await _userService.GetUserByEmailAsync(email);
        if (result.IsFailed){
            return NotFound(new
            {
                Message = "User not found",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        return Ok(new UserResponse(
            Id: result.Value.Id.Value.ToString(),
            FirstName: result.Value.FirstName,
            LastName: result.Value.LastName,
            Email: result.Value.Email,
            Phone: result.Value.Phone,
            Role: result.Value.Role,
            IsActive: result.Value.IsActive,
            CreatedAt: result.Value.CreatedDateTime,
            UpdatedAt: result.Value.UpdatedDateTime));
    }

    [HttpGet("search/phone/{phone}")]
    public async Task<IActionResult> GetUserByPhone(string phone)
    {
        var result = await _userService.GetUserByPhoneAsync(phone);
        if (result.IsFailed)
        {
            return NotFound(new
            {
                Message = "User not found",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        return Ok(new UserResponse(
            Id: result.Value.Id.Value.ToString(),
            FirstName: result.Value.FirstName,
            LastName: result.Value.LastName,
            Email: result.Value.Email,
            Phone: result.Value.Phone,
            Role: result.Value.Role,
            IsActive: result.Value.IsActive,
            CreatedAt: result.Value.CreatedDateTime,
            UpdatedAt: result.Value.UpdatedDateTime));
    }

    [HttpGet("search/id/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        if (!Guid.TryParse(id, out var userIdGuid)) return BadRequest("Invalid UserId format.");

        var userId = UserId.Create(userIdGuid);
        var result = await _userService.GetUserByIdAsync(userId);

        if (result.IsFailed)
        {
            return NotFound(new
            {
                Message = "User not found",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        return Ok(new UserResponse(
            Id: result.Value.Id.Value.ToString(),
            FirstName: result.Value.FirstName,
            LastName: result.Value.LastName,
            Email: result.Value.Email,
            Phone: result.Value.Phone,
            Role: result.Value.Role,
            IsActive: result.Value.IsActive,
            CreatedAt: result.Value.CreatedDateTime,
            UpdatedAt: result.Value.UpdatedDateTime));
    }

    [HttpGet("search/All")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userService.GetUsersAsync();
        return Ok(result.Value.Select(user => new UserResponse(
            Id: user.Id.Value.ToString(),
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Phone: user.Phone,
            Role: user.Role,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedDateTime,
            UpdatedAt: user.UpdatedDateTime)));
    }

    [HttpPut("UpdateUser/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var existingUserResult = await _userService.GetUserByIdAsync(UserId.Create(id));
        if (existingUserResult.IsFailed)
            return NotFound(new
            {
                Message = "User not found",
                Errors = existingUserResult.Errors.Select(e => e.Message)
            });

        var existingUser = existingUserResult.Value;

        existingUser.Update(request.Email, request.Phone, request.Password, existingUser.Role, request.SecurityQuestion,
        request.SecurityAnswer);

        var updateResult = await _userService.UpdateAsync(existingUser);
        if (updateResult.IsFailed)
            return BadRequest(new
            {
                Message = "User update failed",
                Errors = updateResult.Errors.Select(e => e.Message)
            });

        return Ok(new { Message = "User updated successfully." });
    }


}

