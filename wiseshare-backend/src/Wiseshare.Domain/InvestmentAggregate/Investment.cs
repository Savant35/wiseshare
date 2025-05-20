using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.InvestmentAggregate;
public sealed class Investment : AggregateRoot<InvestmentId, string>
{
    public UserId UserId { get; private set; }
    public PropertyId PropertyId { get; private set; }
    public PortfolioId PortfolioId { get; private set; }

    public int NumOfSharesPurchased { get; private set; }
    public decimal InvestmentAmount { get; private set; }
    public float DivedendEarned { get; private set; }
    public bool IsSellPending { get; private set; }
    public int PendingSharesToSell { get; private set; }
    public decimal MarketValue { get; private set; }

    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    private Investment(UserId userId, PropertyId propertyId, PortfolioId portfolioId, int numOfSharesPurchased, decimal investmentAmount)
        : base(InvestmentId.CreateUnique(userId, propertyId))
    {
        UserId = userId;
        PropertyId = propertyId;
        PortfolioId = portfolioId;
        NumOfSharesPurchased = numOfSharesPurchased;
        InvestmentAmount = investmentAmount;
        DivedendEarned = 0f;
        CreatedDateTime = DateTime.UtcNow.ToLocalTime();
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    public static Investment Create(
        UserId userId,
        PropertyId propertyId,
        PortfolioId portfolioId,
        int numOfShares,
        decimal investmentAmount)
    {
        if (numOfShares <= 0)
            throw new ArgumentException("Number of shares must be positive.", nameof(numOfShares));
        if (investmentAmount <= 0)
            throw new ArgumentException("Investment amount must be positive.", nameof(investmentAmount));

        return new Investment(userId, propertyId, portfolioId, numOfShares, investmentAmount);
    }

    public void AddShares(int additionalShares, decimal additionalAmount)
    {
        if (additionalShares <= 0)
            throw new ArgumentException("Additional shares must be positive.", nameof(additionalShares));
        if (additionalAmount <= 0)
            throw new ArgumentException("Additional amount must be positive.", nameof(additionalAmount));

        NumOfSharesPurchased += additionalShares;
        InvestmentAmount += additionalAmount;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    public void SubtractShares(int sharesToSubtract)
    {
        if (sharesToSubtract <= 0)
            throw new ArgumentException("Shares to subtract must be positive.", nameof(sharesToSubtract));
        if (sharesToSubtract > NumOfSharesPurchased)
            throw new InvalidOperationException("Cannot subtract more shares than are currently held.");

        decimal unitCost = InvestmentAmount / NumOfSharesPurchased;
        NumOfSharesPurchased -= sharesToSubtract;
        InvestmentAmount -= unitCost * sharesToSubtract;
        UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
    }

    public void RequestSell(int sharesToSell)
    {
        if (sharesToSell <= 0)
            throw new InvalidOperationException("Requested shares to sell must be positive.");

        int totalPending = PendingSharesToSell + sharesToSell;

        if (totalPending > NumOfSharesPurchased)
            throw new InvalidOperationException("Not enough shares owned to sell.");

        // Update the pending shares with the accumulated value.
        PendingSharesToSell = totalPending;
        IsSellPending = true;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void ApproveSell()
    {
        if (!IsSellPending)
            throw new InvalidOperationException("No pending sell request.");
        if (PendingSharesToSell <= 0)
            throw new InvalidOperationException("Pending shares to sell must be > 0.");
        if (PendingSharesToSell > NumOfSharesPurchased)
            throw new InvalidOperationException("Pending shares exceed owned shares.");

        //SubtractShares(PendingSharesToSell);
        PendingSharesToSell = 0;
        IsSellPending = false;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void RecalculateMarketValue(decimal sharePrice)
    {
        MarketValue = sharePrice * NumOfSharesPurchased;
        UpdatedDateTime = DateTime.UtcNow;
    }

}
