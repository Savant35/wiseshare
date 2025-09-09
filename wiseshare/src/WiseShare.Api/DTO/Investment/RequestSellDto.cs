namespace Wiseshare.Api.DTO.Investment
{
    public record RequestSellDto(
        string UserId,
        string PropertyId,
        int SharesToSell
    );
}
