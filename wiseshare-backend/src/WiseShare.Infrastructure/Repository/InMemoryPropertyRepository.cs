using FluentResults;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Application.Repository;

namespace WiseShare.Infrastructure.Repository;

public class InMemoryPropertyRepository : IPropertyRepository
{
    private readonly List<Property> _properties = new();

    public Result<Property> GetPropertyById(PropertyId propertyId)
    {
        var property = _properties.FirstOrDefault(p => p.Id == propertyId);
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found.");
    }

    public Result<Property> GetPropertyByAddress(string address)
    {
        var property = _properties.FirstOrDefault(p => p.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found.");
    }
    public Result<Property> GetPropertyByName(string name)
    {
        var property = _properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found.");
    }

    public Result<IEnumerable<Property>> GetPropertyByLocation(string location)
    {
        var properties = _properties.Where(p => p.Location.Equals(location, StringComparison.OrdinalIgnoreCase));
        return properties.Any()
            ? Result.Ok(properties)
            : Result.Fail<IEnumerable<Property>>("No properties found in the specified location.");
    }

    public Result<IEnumerable<Property>> GetProperties()
    {
        return Result.Ok((IEnumerable<Property>)_properties);
    }

    public Result Insert(Property property)
    {
        if (_properties.Any(p => p.Id == property.Id))
        {
            return Result.Fail("Property already exists.");
        }

        _properties.Add(property);
        return Result.Ok();
    }

    public Result Delete(PropertyId propertyId)
    {
        var property = _properties.FirstOrDefault(p => p.Id == propertyId);
        if (property is null)
        {
            return Result.Fail("Property not found.");
        }

        _properties.Remove(property);
        return Result.Ok();
    }

    public Result Update(Property property)
    {
        throw new NotImplementedException();
    }
}
