namespace Wiseshare.Api.DTO.Investment;
public record SellInvestmentRequest(
    string UserId,
    string PropertyId,
    int NumberOfSharesToSell
);
