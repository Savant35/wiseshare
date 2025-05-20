using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Admin;
using Wiseshare.Api.DTO.Payment;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Application.Common.Payment;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Application.Services;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase{
    private readonly IUserService _userService;
    private readonly IPropertyService _propertyService;
    private readonly IPaymentService _paymentService;

    public AdminController(IUserService userService, IPropertyService propertyService, IPaymentService paymentService){
        _userService = userService;
        _propertyService = propertyService;
        _paymentService = paymentService;
    }

    // PUT /api/admin/properties/{id}
    [HttpPut("properties/{id:guid}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] AdminUpdatePropertyRequest request)
    {
        var propResult = await _propertyService.GetPropertyByIdAsync(PropertyId.Create(id));
        if (propResult.IsFailed) return BadRequest(propResult.Errors.Select(e => e.Message));
        var property = propResult.Value;
        if (property is null) return NotFound("Property not found.");

        if (!string.IsNullOrWhiteSpace(request.Name)) property.UpdateName(request.Name);
        if (!string.IsNullOrWhiteSpace(request.Address)) property.UpdateAddress(request.Address);
        if (!string.IsNullOrWhiteSpace(request.Location)) property.UpdateLocation(request.Location);
        if (request.OriginalValue.HasValue) property.UpdateOriginalValue(request.OriginalValue.Value);
        if (request.CurrentValue.HasValue) property.UpdateCurrentValue(request.CurrentValue.Value);
        if (request.AvailableShares.HasValue) property.UpdateAvailableShares(request.AvailableShares.Value);
        if (!string.IsNullOrWhiteSpace(request.Description)) property.UpdateDescription(request.Description);
        if (request.InvestmentsEnabled.HasValue)
            if (request.InvestmentsEnabled.Value) property.EnableInvestments();
            else property.DisableInvestments();

        var updateResult = await _propertyService.UpdateAsync(property);
        if (updateResult.IsFailed) return BadRequest(updateResult.Errors.Select(e => e.Message));
        return Ok("Property updated successfully.");
    }

    // DELETE /api/admin/properties/{id}
    [HttpDelete("properties/{id:guid}")]
    public async Task<IActionResult> DeleteProperty(Guid id)
    {
        var deleteResult = await _propertyService.DeleteAsync(PropertyId.Create(id));
        if (deleteResult.IsFailed) return BadRequest(deleteResult.Errors.Select(e => e.Message));
        return Ok("Property deleted successfully.");
    }

    // PUT /api/admin/users/{id}
    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateAdminRequest request){
        var userResult = await _userService.GetUserByIdAsync(UserId.Create(id));
        if (userResult.IsFailed) return BadRequest(userResult.Errors.Select(e => e.Message));
        var user = userResult.Value;
        if (user is null) return NotFound("User not found.");

        user.Update(
            request.Email,
            request.Phone,
            request.Password,
            request.Role,
            request.SecurityQuestion,
            request.SecurityAnswer
        );

        var updateResult = await _userService.UpdateAsync(user);
        if (updateResult.IsFailed) return BadRequest(updateResult.Errors.Select(e => e.Message));
        return Ok("User updated successfully.");
    }

    // POST /api/admin/users/{id}/deactivate
    [HttpPost("users/{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _userService.DeactivateUserAsync(UserId.Create(id));
        if (result.IsFailed) return BadRequest(result.Errors.Select(e => e.Message));
        return Ok("Account deactivated successfully.");
    }

    // POST /api/admin/users/{id}/reactivate
    [HttpPost("users/{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        var result = await _userService.ReactivateUserAsync(UserId.Create(id));
        if (result.IsFailed) return BadRequest(result.Errors.Select(e => e.Message));
        return Ok("Account reactivated successfully.");
    }

    [HttpPost("{id:guid}/disable-investments")]
    public async Task<IActionResult> DisableInvestments(Guid id)
    {
        var result = await _propertyService
            .SetInvestmentsStatusAsync(PropertyId.Create(id), investmentsEnabled: false);
        if (result.IsFailed)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        return Ok(new { Message = $"Investments disabled for property {id}" });
    }

    // POST api/admin/property/{id}/enable-investments
    [HttpPost("{id:guid}/enable-investments")]
    public async Task<IActionResult> EnableInvestments(Guid id){
        var result = await _propertyService
            .SetInvestmentsStatusAsync(PropertyId.Create(id), investmentsEnabled: true);
        if (result.IsFailed)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        return Ok(new { Message = $"Investments enabled for property {id}" });
    }

    // GET /api/admin/payments
    [HttpGet("payments")]
    public async Task<IActionResult> GetAllPayments(){
        var result = await _paymentService.GetAllPaymentsAsync();
        if (result.IsFailed || !result.Value.Any())
            return NotFound("No payments found.");

        var response = result.Value.Select(p => new PaymentResponse(
            Id: p.Id.Value.ToString(),
            UserId: p.UserId.Value.ToString(),
            Amount: p.Amount,
            Type: p.Type.ToString(),
            Status: p.Status.ToString(),
            StripePaymentIntentId: p.StripePaymentIntentId,
            CreatedAt: p.CreatedDateTime,
            UpdatedAt: p.UpdatedDateTime
        ));
        return Ok(response);
    }

    // POST /api/admin/payments/search
    [HttpPost("payments/search")]
    public async Task<IActionResult> GetFilteredPayments([FromBody] PaymentFilter filter){
        var result = await _paymentService.GetFilteredPaymentsAsync(filter);
        if (result.IsFailed || !result.Value.Any())
            return NotFound("No payments match the given filters.");

        var response = result.Value.Select(p => new PaymentResponse(
            Id: p.Id.Value.ToString(),
            UserId: p.UserId.Value.ToString(),
            Amount: p.Amount,
            Type: p.Type.ToString(),
            Status: p.Status.ToString(),
            StripePaymentIntentId: p.StripePaymentIntentId,
            CreatedAt: p.CreatedDateTime,
            UpdatedAt: p.UpdatedDateTime
        ));
        return Ok(response);
    }
}
