using System.Dynamic;
using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;



namespace Wiseshare.Domain.PropertyAggregate;

public sealed class Property : AggregateRoot<PropertyId, Guid>{
    public string Description{get; private set;}
    public string Address {get; private set;}
    public string Location {get; private set;}
    public double OriginalValue{get; private set;}
    public double CurrentValue{get; private set;}
    public double SharePrice {get; private set;}
    public int AvailableShares {get; private set;}
    public string Name {get; private set;}

    public DateTime CreatedDateTime {get; private set;}
    public DateTime UpdatedDateTime {get; private set;}

    private Property(string name, string address, string location, double originalValue, double currentValue, double sharePrice,
                     int availableShares, string description, PropertyId? propertyId = null)
            : base(propertyId ?? PropertyId.CreateUnique())
    {
        Address = address;
        Location = location;
        OriginalValue = originalValue;
        CurrentValue = currentValue;
        SharePrice = sharePrice;
        AvailableShares = availableShares;
        Description = description;
        Name = name;
    }
    public static Property Create(string name, string address,string location, double originalValue, double currentValue,double sharePrice, int availableShares, string description)
    {
        return new Property(name, address,location,originalValue, currentValue,sharePrice,availableShares,description);

    }
}