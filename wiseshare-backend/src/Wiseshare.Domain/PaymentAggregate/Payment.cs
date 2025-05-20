using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.PaymentAggregate;

public enum PaymentStatus { Pending, Completed, Failed }

public enum PaymentType { Deposit, Withdrawal, Investment, Refund }

public sealed class Payment : AggregateRoot<PaymentId, Guid>{
    public UserId UserId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentType Type { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? StripePaymentIntentId { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    private Payment(UserId userId, decimal amount, PaymentType type, PaymentStatus status, string? stripePaymentIntentId = null,
    PaymentId? paymentId = null) : base(paymentId ?? PaymentId.CreateUnique()){
        UserId = userId;
        Amount = amount;
        Type = type;
        Status = status;
        StripePaymentIntentId = stripePaymentIntentId;
        CreatedDateTime = DateTime.UtcNow.ToLocalTime();
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    // Factory methods
    public static Payment CreateDeposit(UserId userId, decimal amount, string stripePaymentIntentId){
        return new Payment(userId, amount, PaymentType.Deposit, PaymentStatus.Pending, stripePaymentIntentId);
    }

    public static Payment CreateWithdrawal(UserId userId, decimal amount){
        return new Payment(userId, amount, PaymentType.Withdrawal, PaymentStatus.Pending);
    }

    public static Payment CreateInvestment(UserId userId, decimal amount){
        // If user invests from wallet, we set it completed immediately
        return new Payment(userId, amount, PaymentType.Investment, PaymentStatus.Completed);
    }
    public static Payment CreateRefund(UserId userId, decimal amount, string intentId){ 
        return new Payment(userId, amount, PaymentType.Refund, PaymentStatus.Completed, intentId);
    }


    // Status updates
    public void MarkAsCompleted(){
        Status = PaymentStatus.Completed;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    public void MarkAsFailed(){
        Status = PaymentStatus.Failed;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

#pragma warning disable CS8618
    private Payment()
    {
    }
#pragma warning restore CS8618
}
