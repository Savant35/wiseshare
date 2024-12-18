
using Wiseshare.Domain.Common.Models;

namespace Wiseshare.Domain.UserAggregate.ValueObjects;

//summary: create unique IDs for users
public sealed class UserId : AggregateRootId<Guid> //: indicates inheritance userid is a special version of AggregateRootID can can use its properties and methods
{
    private UserId(Guid value) : base(value)
    {
    }

    public static UserId CreateUnique()//
    {
        return new UserId(Guid.NewGuid());//generates a new global unique identifier 128 bit
    }

    public static UserId Create(Guid userId)//accepts existing Guid to create userID
    {
        return new UserId(userId);
    }
}


