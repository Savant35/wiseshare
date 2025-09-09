using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.InvestmentAggregate.ValueObject;

public sealed class InvestmentId : AggregateRootId<string>
{
    private InvestmentId(string value) : base(value) { }

    private InvestmentId(UserId userId, PropertyId propertyId)
        : base($"Bill_{userId.Value}_{propertyId.Value}") { }

    public static InvestmentId CreateUnique(UserId userId, PropertyId propertyId)
    {
        return new InvestmentId(userId, propertyId);
    }

    public static InvestmentId Create(string value)
    {
        return new InvestmentId(value);
    }
}
