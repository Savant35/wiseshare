namespace Wiseshare.Api.DTO.Payment{
    public record DepositRequest(
        string UserId,   
        decimal Amount   
    );
}
