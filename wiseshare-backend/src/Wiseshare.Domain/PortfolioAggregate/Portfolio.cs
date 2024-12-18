using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.InvestmentAgrregate.ValueObject;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;

namespace Wiseshare.domain.PortfolioAggregate;

public sealed class Portfolio : AggregateRoot<PortfolioId,string> {
    public List<Investment> Investment {get; private set;}
    public DateTime CreatedDateTime {get; private set;}
    public DateTime UpdatedDateTime {get; private set;}


    

}

