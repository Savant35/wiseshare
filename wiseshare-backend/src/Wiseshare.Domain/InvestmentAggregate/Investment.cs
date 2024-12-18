using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.InvestmentAgrregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

public sealed class Investment : AggregateRoot<InvestmentId,string>{
    public UserId UserId {get; private set;}
    public PropertyId PropertyId {get; private set;}
    public int NumOfSharesPurchased {get; private set;}

    public DateTime CreatedDateTime {get; private set;}
    public DateTime UpdatedDateTime {get; private set;}




    private Investment(UserId userId, PropertyId propertyId, int numOfSharesPurchased, decimal investmentAmount,
    float divedendEarned){
        UserId = userId;
        PropertyId = propertyId;
        NumOfSharesPurchased = numOfSharesPurchased;
    }

    private static Investment create(UserId userId, PropertyId propertyId, int numOfSharesPurchased,
     decimal investmentAmount,float divedendEarned){
        return new Investment(userId,propertyId,numOfSharesPurchased,investmentAmount,divedendEarned);
     }

}
