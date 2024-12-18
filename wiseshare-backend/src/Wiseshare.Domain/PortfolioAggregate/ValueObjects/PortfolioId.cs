using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace wiseshare.Domain.PortfolioAggregate.ValueObjects;

public sealed class PortfolioId : AggregateRootId<string> {

    private PortfolioId(string value) : base(value){}

    private PortfolioId(UserId userId)
        : base($"Portfolio_{userId.Value}") {}

    public static PortfolioId createUnique(UserId userId){
        return new PortfolioId(userId);
    }
    public static PortfolioId Create(string value){
        return new PortfolioId(value);
    }
}