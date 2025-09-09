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
    public decimal RealizedProfit { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }


    private Portfolio(UserId userId){
        UserId = userId;
        Investment = [];
        TotalInvestmentAmount = 0;
        RealizedProfit = 0m;
        Id = PortfolioId.CreateUnique(userId);
    }

    public static Portfolio Create(UserId userId){
        return new Portfolio(userId);
    }
    public void IncreaseTotalInvestmentAmount(decimal amount){
        if (amount < 0){
            throw new InvalidOperationException("Cannot increase by a negative amount.");
        }
        TotalInvestmentAmount += amount;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }
    public void DecreaseTotalInvestmentAmount(decimal amount)
    {

        if (amount < 0)
        {
            throw new InvalidOperationException("Amount to decrease must be positive.");
        }
        if (TotalInvestmentAmount < amount)
        {
            throw new InvalidOperationException("Cannot decrease more than the current total investment amount.");
        }
        TotalInvestmentAmount -= amount;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    public void AdjustRealizedProfit(decimal profitOrLoss){
        RealizedProfit += profitOrLoss;
        UpdatedDateTime = DateTime.UtcNow;
    }
    

}

