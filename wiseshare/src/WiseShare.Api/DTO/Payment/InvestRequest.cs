namespace Wiseshare.Api.DTO.Payment
{
    public record InvestRequest(
        string UserId,     
        string PropertyId,  
        int NumberOfShares 
    );
}
