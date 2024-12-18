using Wiseshare.Domain.Common.Models;

namespace Wiseshare.Domain.PropertyAggregate.ValueObjects;
public sealed class PropertyId : AggregateRootId<Guid> {
    private PropertyId(Guid value) : base(value){}

    public static PropertyId CreateUnique() 
    {
        return new PropertyId(Guid.NewGuid());
    }

    public static PropertyId Create(Guid propertyId){
        return new PropertyId(propertyId);
    }
}