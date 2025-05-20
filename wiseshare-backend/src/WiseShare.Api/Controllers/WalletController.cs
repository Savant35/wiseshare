using Microsoft.AspNetCore.Mvc;
using Wiseshare.Application.Services;
using Wiseshare.Domain.UserAggregate.ValueObjects;

    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase{
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService){
            _walletService = walletService;
        }

        // Returns the wallet balance for a given user.
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWalletBalance(string userId){
            if (!Guid.TryParse(userId, out var guid))
                return BadRequest("Invalid UserId format.");

            var result = await _walletService.GetWalletByUserIdAsync(UserId.Create(guid));
            if (result.IsFailed)
                return NotFound(result.Errors.Select(e => e.Message));

            var wallet = result.Value;
            return Ok(new{
                UserId = wallet.UserId.Value.ToString(),
                Balance = wallet.Balance,
                UpdatedAt = wallet.UpdatedDateTime
            });
        }
    }
