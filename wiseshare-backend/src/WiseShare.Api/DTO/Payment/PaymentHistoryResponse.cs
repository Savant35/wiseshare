namespace Wiseshare.Api.DTO.Payment;
    public record PaymentHistoryResponse(
        string Id,                   
        decimal Amount,
        string Type,                  
        string Status,                
        DateTime CreatedAt,
        DateTime UpdatedAt,
        string? PaymentIntentId
    );
