using Wiseshare.Domain.Common.Models;

namespace Wiseshare.Domain.PaymentAggregate.ValueObjects;

public sealed class PaymentId : AggregateRootId<Guid>
{
    private PaymentId(Guid value) : base(value)
    {
    }

    public static PaymentId CreateUnique()
    {
        return new PaymentId(Guid.NewGuid());
    }

    public static PaymentId Create(Guid paymentId)
    {
        return new PaymentId(paymentId);
    }
}
