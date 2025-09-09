namespace Wiseshare.Api.DTO.Payment
{
    public record WithdrawRequest(
        string UserId,   
        decimal Amount   
    );
}
