using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Domain.InvestmentAgrregate.ValueObject;

public sealed class InvestmentId : AggregateRootId<string> {
    
    private  InvestmentId(string value) : base(value){
        
    }

     private InvestmentId(UserId userId, PropertyId propertyId)
        : base($"Bill_{userId.Value}_{propertyId.Value}")//returns "Bill"_userId_propertyID
    {
    }
    public static InvestmentId Create(UserId userId, PropertyId propertyId){
        return new InvestmentId(userId, propertyId);
    }

    public static InvestmentId create(string value){
        return new InvestmentId(value);
    }
        
    }
