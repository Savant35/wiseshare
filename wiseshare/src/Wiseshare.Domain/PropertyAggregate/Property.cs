using Wiseshare.Domain.Common.Models;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Domain.PropertyAggregate;

public sealed class Property : AggregateRoot<PropertyId, Guid>{
    public string Description { get; private set; }
    public string Address { get; private set; }
    public string Location { get; private set; }
    public double OriginalValue { get; private set; }
    public double CurrentValue { get; private set; }
    public double SharePrice { get; private set; }
    public int AvailableShares { get; private set; }
    public string Name { get; private set; }
    public string ImageUrls { get; private set; }
    public bool InvestmentsEnabled { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    private Property(string name, string address, string location, double originalValue, string description,
         PropertyId? propertyId = null) : base(propertyId ?? PropertyId.CreateUnique())
    {
        Name = name;
        Address = address;
        Location = location;
        OriginalValue = originalValue;
        CurrentValue = originalValue;
        SharePrice = Math.Round(OriginalValue / 20000, 2); // Calculate share price
        AvailableShares = 20000; // Default to 20000 shares
        Description = description;
        ImageUrls = $"/images/{Id.Value}/";
        InvestmentsEnabled = true;
    }
    public static Property Create(string name, string address, string location, double originalValue, string description)
    {
        return new Property(name, address, location, originalValue, description);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
        UpdatedDateTime = DateTime.UtcNow;
    }
    public void UpdateAddress(string newAddress)
    {
        Address = newAddress;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void UpdateLocation(string newLocation)
    {
        Location = newLocation;
        UpdatedDateTime = DateTime.UtcNow;
    }
    public void UpdateOriginalValue(double newOriginalValue)
    {
        if (newOriginalValue < 0)
            throw new InvalidOperationException("OriginalValue cannot be negative.");

        // remember what original used to be
        var oldOriginal = OriginalValue;

        OriginalValue = newOriginalValue;

        // only bump current along if nobody’s ever changed it
        if (CurrentValue == oldOriginal)
            CurrentValue = newOriginalValue;

        SharePrice = Math.Round(OriginalValue / AvailableShares, 2);
        UpdatedDateTime = DateTime.UtcNow;
    }



    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void UpdateAvailableShares(int newAvailableShares)
    {

        if (newAvailableShares < 0)
        {
            throw new InvalidOperationException("AvailableShares cannot be negative.");
        }

        AvailableShares = newAvailableShares;
        UpdatedDateTime = DateTime.UtcNow;
    }
    public void UpdateImageUrls(string newFolderUrl)
    {
        ImageUrls = newFolderUrl;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void DisableInvestments()
    {
        InvestmentsEnabled = false;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void EnableInvestments()
    {
        InvestmentsEnabled = true;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public void UpdateCurrentValue(double newValue)
    {
        if (newValue < 0) throw new InvalidOperationException("Value must be ≥ 0");
        CurrentValue = newValue;
        SharePrice = Math.Round(CurrentValue / AvailableShares, 2);
        UpdatedDateTime = DateTime.UtcNow;
    }





#pragma warning disable CS8618 //disable warning for non-nullable since i have a nullable value in constructor
    private Property()
    {
    }
#pragma warning restore CS8618
}