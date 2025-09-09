namespace Wiseshare.Api.DTO.Portfolio;

public record PortfolioResponse(
  string Id,
  string UserId,
  decimal TotalInvestmentAmount,
  decimal PortfolioValue,
  decimal PortfolioProfit,
  decimal RealizedProfit,
  decimal AllTimeProfit,
  List<string> Investments
);
