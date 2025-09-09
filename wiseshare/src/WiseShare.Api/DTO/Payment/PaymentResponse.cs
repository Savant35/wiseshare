namespace Wiseshare.Api.DTO.Payment;
    public record PaymentResponse(
        string Id,                   
        string UserId,                
        decimal Amount,
        string Type,                  
        string Status,                
        string? StripePaymentIntentId,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
