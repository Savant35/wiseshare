using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Domain.InvestmentAggregate;

namespace Wiseshare.domain.PortfolioAggregate;

public sealed class Portfolio : AggregateRoot<PortfolioId, string>
{
    public List<Investment> Investment { get; private set; }
    public UserId UserId { get; private set; }
    public decimal TotalInvestmentAmount { get; private set; }
    public decimal TotalReturns { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }


    private Portfolio(UserId userId)
    {
        UserId = userId;
        Investment = [];
        TotalInvestmentAmount = 0;
        TotalReturns = 0;
         Id = PortfolioId.CreateUnique(userId);
    }

    public static Portfolio Create(UserId userId)
    {
        return new Portfolio(userId);
    }

}

