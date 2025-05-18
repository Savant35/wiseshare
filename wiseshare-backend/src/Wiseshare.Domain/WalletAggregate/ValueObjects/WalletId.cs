using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.WalletAggregate.ValueObjects;

public sealed class WalletId : AggregateRootId<string>
{
    // Private constructor to enforce the use of factory methods
    private WalletId(string value) : base(value)
    {
    }

    // Factory method to create WalletId using UserId
    public static WalletId CreateUnique(UserId userId)
    {
        // Generates a unique WalletId by appending "Wallet_" to the UserId value
        return new WalletId($"Wallet_{userId.Value}");
    }

    // Factory method to create WalletId using a raw string value
    public static WalletId Create(string value)
    {
        return new WalletId(value);
    }
}
