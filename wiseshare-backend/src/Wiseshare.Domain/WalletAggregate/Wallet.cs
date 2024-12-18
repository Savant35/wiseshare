using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.WalletAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Domain.WalletAggregate;

public sealed class Wallet : AggregateRoot<WalletId, string> {

    public UserId UserId{get; private set;}
    public decimal Balance {get; private set;}

    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }



private Wallet(UserId userId, DateTime createdDateTime, DateTime updatedDateTime)
{
    UserId = userId ?? throw new ArgumentNullException(nameof(userId)); //?? ensures userId is not null. if it is throw an eception
    Balance = 0;
    CreatedDateTime = createdDateTime;
    UpdatedDateTime = updatedDateTime;
}
public static Wallet Create(UserId userId, DateTime createdDateTime, DateTime updatedDateTime)
{
    return new Wallet(userId,  createdDateTime, updatedDateTime);
}

}

