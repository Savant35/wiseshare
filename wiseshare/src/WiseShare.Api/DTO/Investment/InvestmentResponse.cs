namespace Wiseshare.Api.DTO.Investments;
public record InvestmentResponse(
    string Id,
    string UserId,
    string PropertyId,
    int NumOfSharesPurchased,
    decimal InvestmentAmount,
    decimal SharePrice,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
