using Microsoft.AspNetCore.Mvc;
using Stripe;
using Wiseshare.Api.DTO.Payment;
using Wiseshare.Application.Services;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;

namespace Wiseshare.Api.Controllers;
[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public PaymentController(IPaymentService paymentService, IConfiguration configuration)
    {
        _paymentService = paymentService;
        _configuration = configuration;
    }

    [HttpGet("search/user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUser(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return BadRequest("Invalid UserId format.");

        var result = await _paymentService.GetPaymentsByUserAsync(UserId.Create(userGuid));
        if (result.IsFailed)
        {
            return NotFound(new
            {
                Message = "No payments found for this user",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        var response = result.Value.Select(p => new PaymentHistoryResponse(
            Id: p.Id.Value.ToString(),
            Amount: p.Amount,
            Type: p.Type.ToString(),
            Status: p.Status.ToString(),
            CreatedAt: p.CreatedDateTime,
            UpdatedAt: p.UpdatedDateTime,
            PaymentIntentId: p.StripePaymentIntentId
        ));

        return Ok(response);
    }

    [HttpGet("search/id/{id}")]
    public async Task<IActionResult> GetPaymentById(string id)
    {
        if (!Guid.TryParse(id, out var paymentGuid))
            return BadRequest("Invalid PaymentId format.");

        var result = await _paymentService.GetPaymentByIdAsync(PaymentId.Create(paymentGuid));
        if (result.IsFailed)
        {
            return NotFound(new
            {
                Message = "Payment not found",
                Errors = result.Errors.Select(e => e.Message)
            });
        }

        var p = result.Value;
        return Ok(new PaymentHistoryResponse(
            Id: p.Id.Value.ToString(),
            Amount: p.Amount,
            Type: p.Type.ToString(),
            Status: p.Status.ToString(),
            CreatedAt: p.CreatedDateTime,
            UpdatedAt: p.UpdatedDateTime,
            PaymentIntentId: p.StripePaymentIntentId
        ));
    }


    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest req)
    {
        var result = await _paymentService.DepositAsync(UserId.Create(Guid.Parse(req.UserId)), req.Amount);
        if (result.IsFailed)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });

        var value = result.Value;
        if (!string.IsNullOrEmpty(value.OnboardingUrl))
        {
            // onboarding needed
            return Ok(new { value.Message, value.OnboardingUrl });
        }

        // fully onboarded
        return Ok(new
        {
            PaymentId = value.PaymentId,
            ClientSecret = value.ClientSecret
        });
    }
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawStripeRequest req)
    {
        // parse & validate
        if (!Guid.TryParse(req.UserId, out var userGuid))
            return BadRequest("Invalid UserId format.");

        // call your new two-param WithdrawAsync
        var result = await _paymentService.WithdrawAsync(
            UserId.Create(userGuid),
            req.Amount
        );

        if (result.IsFailed)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });

        // success: return the Payment record’s ID
        return Ok(new { PaymentId = result.Value.Id.Value.ToString() });
    }




    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromBody] RefundRequest request)
    {
        var userGuid = Guid.Parse(request.UserId);
        var result = await _paymentService.RefundAsync(UserId.Create(userGuid), request.Amount, request.PaymentIntentId);
        if (result.IsFailed) return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        var payment = result.Value;
        return Ok(new { PaymentId = payment.Id.Value.ToString() });
    }

    [HttpPost("invest")]
    public async Task<IActionResult> Invest([FromBody] InvestRequest request)
    {
        var userGuid = Guid.Parse(request.UserId);
        var propertyGuid = Guid.Parse(request.PropertyId);
        var result = await _paymentService.InvestAsync(UserId.Create(userGuid), PropertyId.Create(propertyGuid), request.NumberOfShares);
        if (result.IsFailed)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        var payment = result.Value;
        return Ok(new { PaymentId = payment.Id.Value.ToString() });
    }

    [HttpPost("stripe-webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        // allow body to be read multiple times
        HttpContext.Request.EnableBuffering();
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        HttpContext.Request.Body.Position = 0;

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _configuration["Stripe:WebhookSecret"],
            throwOnApiVersionMismatch: false
        );

        if (stripeEvent.Type == "payment_intent.succeeded" &&
            stripeEvent.Data.Object is PaymentIntent succeeded)
        {
            await _paymentService.UpdateDepositStatusAsync(succeeded.Id, true);
        }
        else if (stripeEvent.Type == "payment_intent.payment_failed" &&
                 stripeEvent.Data.Object is PaymentIntent failed)
        {
            await _paymentService.UpdateDepositStatusAsync(failed.Id, false);
        }

        return Ok();
    }

}

public record WithdrawStripeRequest(string UserId, decimal Amount);
public record RefundRequest(string UserId, decimal Amount, string PaymentIntentId);
