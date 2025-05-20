using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Investment;
using Wiseshare.Api.DTO.Investments;
using Wiseshare.Application.services.InvestmentServices;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Api.Controllers;
[ApiController]
[Route("api/investment")]
public class InvestmentController : ControllerBase
{
    private readonly IInvestmentService _investmentService;

    public InvestmentController(IInvestmentService investmentService)
    {
        _investmentService = investmentService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetInvestmentsByUser(string userId)
    {
        if (!Guid.TryParse(userId, out var guid))
            return BadRequest("Invalid UserId format.");

        var result = await _investmentService.GetInvestmentByUserIdAsync(UserId.Create(guid));
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));

        var response = result.Value.Select(inv =>
        {
            var shares = inv.NumOfSharesPurchased;
            var amount = inv.InvestmentAmount;
            var price = shares > 0 ? amount / shares : 0m;
            return new InvestmentResponse(
                Id: inv.Id.Value.ToString(),
                UserId: inv.UserId.Value.ToString(),
                PropertyId: inv.PropertyId.Value.ToString(),
                NumOfSharesPurchased: shares,
                InvestmentAmount: amount,
                SharePrice: price,
                CreatedAt: inv.CreatedDateTime,
                UpdatedAt: inv.UpdatedDateTime
            );
        });

        return Ok(response);
    }

    // GET: api/investment/property/{propertyId}
    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetInvestmentsByProperty(string propertyId)
    {
        if (!Guid.TryParse(propertyId, out var guid))
            return BadRequest("Invalid PropertyId format.");

        var result = await _investmentService.GetInvestmentByPropertyIdAsync(
            PropertyId.Create(guid));
        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));

        var response = result.Value.Select(inv =>
        {
            var shares = inv.NumOfSharesPurchased;
            var amount = inv.InvestmentAmount;
            var price = shares > 0 ? amount / shares : 0m;
            return new InvestmentResponse(
                Id: inv.Id.Value.ToString(),
                UserId: inv.UserId.Value.ToString(),
                PropertyId: inv.PropertyId.Value.ToString(),
                NumOfSharesPurchased: shares,
                InvestmentAmount: amount,
                SharePrice: price,
                CreatedAt: inv.CreatedDateTime,
                UpdatedAt: inv.UpdatedDateTime
            );
        });

        return Ok(response);
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellShares([FromBody] SellInvestmentRequest request)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
            return BadRequest("Invalid UserId format.");
        if (!Guid.TryParse(request.PropertyId, out var propertyGuid))
            return BadRequest("Invalid PropertyId format.");

        var result = await _investmentService.SellSharesAsync(
            UserId.Create(userGuid),
            PropertyId.Create(propertyGuid),
            request.NumberOfSharesToSell);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        if (result.Value is null)
            return Ok("Investment record deleted as all shares have been sold.");

        var inv = result.Value;
        var shares = inv.NumOfSharesPurchased;
        var amount = inv.InvestmentAmount;
        var price = shares > 0 ? amount / shares : 0m;

        var response = new InvestmentResponse(
            Id: inv.Id.Value.ToString(),
            UserId: inv.UserId.Value.ToString(),
            PropertyId: inv.PropertyId.Value.ToString(),
            NumOfSharesPurchased: shares,
            InvestmentAmount: amount,
            SharePrice: price,
            CreatedAt: inv.CreatedDateTime,
            UpdatedAt: inv.UpdatedDateTime
        );


        return Ok(response);
    }

    [HttpPost("request-sell")]
    public async Task<IActionResult> RequestSellShares([FromBody] RequestSellDto request)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
            return BadRequest("Invalid UserId format.");
        if (!Guid.TryParse(request.PropertyId, out var propertyGuid))
            return BadRequest("Invalid PropertyId format.");

        var result = await _investmentService.RequestSellSharesAsync(
            UserId.Create(userGuid),
            PropertyId.Create(propertyGuid),
            request.SharesToSell);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return Ok("Sell request submitted. Pending admin approval.");
    }

    [HttpPost("approve-sell")]
    public async Task<IActionResult> ApproveSellShares([FromBody] ApproveSellDto request)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
            return BadRequest("Invalid UserId format.");
        if (!Guid.TryParse(request.PropertyId, out var propertyGuid))
            return BadRequest("Invalid PropertyId format.");

        var result = await _investmentService.ApproveSellSharesAsync(
            UserId.Create(userGuid),
            PropertyId.Create(propertyGuid));

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return Ok("Sell request approved. Shares subtracted from the investment.");
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingSells()
    {
        // 1) Fetch all investments
        var allResult = await _investmentService.GetInvestmentsAsync();
        if (allResult.IsFailed)
            return BadRequest(allResult.Errors.Select(e => e.Message));

        // 2) Filter for those with a sell request
        var pending = allResult.Value
            .Where(inv => inv.IsSellPending)  // flags on your domain model :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}
            .Select(inv => new PendingSellDto(
                InvestmentId: inv.Id.Value.ToString(),
                UserId: inv.UserId.Value.ToString(),
                PropertyId: inv.PropertyId.Value.ToString(),
                SharesToSell: inv.PendingSharesToSell,
                RequestedAt: inv.UpdatedDateTime    // or track your own timestamp
            ));

        // 3) Return the array
        return Ok(pending);
    }
}
