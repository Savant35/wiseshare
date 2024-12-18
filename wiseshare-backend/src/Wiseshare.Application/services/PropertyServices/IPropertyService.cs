using FluentResults;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public interface IPropertyService
{
    public Result<Property> GetPropertyById(PropertyId propertyId);
    public Result<Property> GetPropertyByName(string name);

    public Result<Property> GetPropertyByAddress(string address);

    public Result<IEnumerable<Property>> GetPropertyByLocation(string location);

    public Result<IEnumerable<Property>> GetProperties();

    public Result Insert(Property property);
    public Result Delete(PropertyId propertyId);
    public Result Update(Property property);
    
}