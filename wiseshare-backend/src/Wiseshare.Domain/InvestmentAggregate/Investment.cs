using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.InvestmentAggregate;
public sealed class Investment : AggregateRoot<InvestmentId, string>
{
    public UserId UserId { get; private set; }
    public PropertyId PropertyId { get; private set; }
    public int NumOfSharesPurchased { get; private set; }
    public decimal InvestmentAmount { get; private set; } // Set in constructor
    public float DivedendEarned { get; private set; }     // Set in constructor

    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    private Investment(UserId userId, PropertyId propertyId, int numOfSharesPurchased)
    {
        UserId = userId;
        PropertyId = propertyId;
        NumOfSharesPurchased = numOfSharesPurchased;

        
        InvestmentAmount = 0;
        DivedendEarned = 0f;
    }

    public static Investment Create(UserId userId, PropertyId propertyId, int numOfSharesPurchased)
    {
        return new Investment(userId, propertyId, numOfSharesPurchased);
    }
}
