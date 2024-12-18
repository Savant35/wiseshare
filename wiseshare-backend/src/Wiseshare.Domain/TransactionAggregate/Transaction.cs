using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Domain.TransactionAggregate;

public sealed class Transaction : AggregateRoot<TransactionId,string>{

    public UserId UserId {get; private set;}
    public PropertyId PropertyId {get; private set;}
    public string TransactionType {get; private set;}
    public decimal TransactionAmount {get; private set;}
    public string PaymentMethod {get; private set;}
    private Transaction(UserId userId, PropertyId propertyId,string transactionType, decimal transactionAmount
    ,string paymentMethod)
    {
        UserId = userId;
        PropertyId = propertyId;
        TransactionType = transactionType;
        TransactionAmount = transactionAmount;
        PaymentMethod = paymentMethod;
    }

    public static Transaction Create(UserId userID, PropertyId propertyId, string transactionType, decimal transactionAmount,
    string paymentMethod){
        return new Transaction(userID,propertyId,transactionType,transactionAmount,paymentMethod);
    }
}
