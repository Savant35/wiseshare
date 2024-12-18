using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.WalletAggregate.ValueObjects;

public sealed class WalletId : AggregateRootId<string>
{
    // Private constructor to enforce the use of factory methods
    private WalletId(string value) : base(value)
    {
    }

    // Private constructor that generates WalletId based on UserId  If userId.Value is "User123", then $"Wallet_{userId.Value}" evaluates 
    //to "Wallet_User123".This ensures that each WalletId is uniquely tied to a specific user, following a consistent naming convention.
    private WalletId(UserId userId)
        : base($"Wallet_{userId.Value}") 
    {
    }

    // Factory method to create WalletId using UserId
    public static WalletId CreateUnique(UserId userId)
    {
        // Validate that userId is not null or invalid if necessary
        return new WalletId(userId);
    }

    // Factory method to create WalletId using a raw string value
    public static WalletId Create(string value)
    {
        return new WalletId(value);
    }
}
