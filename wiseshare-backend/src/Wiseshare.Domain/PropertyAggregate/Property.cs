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

    private Property(string name, string address, string location, double originalValue, string description, PropertyId? propertyId = null)
            : base(propertyId ?? PropertyId.CreateUnique())
    {
       Name = name;
        Address = address;
        Location = location;
        OriginalValue = originalValue;  
        CurrentValue = originalValue; // CurrentValue is the same as OriginalValue at creation
        SharePrice = Math.Round(OriginalValue / 20000, 2); // Calculate share price
        AvailableShares = 20000; // Default to 20000 shares
        Description = description;
    }
    public static Property Create(string name, string address,string location, double originalValue, string description)
    {
        return new Property(name, address, location, originalValue, description);

    }
    #pragma warning disable CS8618 //disable warning for non-nullable since i have a nullable value in constructor
    private Property()
    {
    }
#pragma warning restore CS8618
}