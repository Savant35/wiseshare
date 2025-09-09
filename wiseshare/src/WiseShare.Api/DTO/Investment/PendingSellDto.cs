// PendingSellDto.cs
namespace Wiseshare.Api.DTO.Investment;
    public record PendingSellDto(
        string InvestmentId,
        string UserId,
        string PropertyId,
        int SharesToSell,
        DateTime RequestedAt
    );
