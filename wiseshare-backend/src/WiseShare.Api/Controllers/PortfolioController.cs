using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Portfolio;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Application.services.PortfolioServices;

namespace Wiseshare.Api.Controllers
{
    [ApiController]
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IPropertyService _propertyService;

        public PortfolioController(IPortfolioService portfolioService, IPropertyService propertyService)
        {
            _portfolioService = portfolioService;
            _propertyService = propertyService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPortfolioByUserId(string userId)
        {
            if (!Guid.TryParse(userId, out var guid))
                return BadRequest("Invalid UserId format.");

            var pfRes = await _portfolioService.GetPortfolioByUserIdAsync(UserId.Create(guid));
            if (pfRes.IsFailed)
                return NotFound(pfRes.Errors.Select(e => e.Message));

            var portfolio = pfRes.Value;

            // 1) look up each share’s current price in parallel
            var valuationTasks = portfolio.Investment.Select(async inv =>
            {
                var pRes = await _propertyService.GetPropertyByIdAsync(
                                PropertyId.Create(inv.PropertyId.Value));
                return (Shares: inv.NumOfSharesPurchased,
                        Price: pRes.IsSuccess ? (decimal?)pRes.Value.SharePrice : null);
            });

            var valuations = await Task.WhenAll(valuationTasks);

            // 2) sum up the market value
            var rawValue = valuations.Sum(v => (v.Price ?? 0m) * v.Shares);

            // 3) compute each metric and round to 2 decimals
            decimal totalInvest = decimal.Round(portfolio.TotalInvestmentAmount, 2);
            decimal portfolioValue = decimal.Round(rawValue, 2);
            decimal unrealized = decimal.Round(portfolioValue - totalInvest, 2);
            decimal realized = decimal.Round(portfolio.RealizedProfit, 2);
            decimal allTimeProfit = decimal.Round(unrealized + realized, 2);

            var response = new PortfolioResponse(
                portfolio.Id.Value.ToString(),
                portfolio.UserId.Value.ToString(),
                totalInvest,
                portfolioValue,
                unrealized,
                realized,
                allTimeProfit,
                portfolio.Investment.Select(i => i.Id.Value.ToString()).ToList()
            );

            return Ok(response);
        }
    }

}
