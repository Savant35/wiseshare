using FluentResults;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.Repository;
public interface IPropertyRepository
{
    public Result<Property> GetPropertyById(PropertyId propertyId);
    public Result<Property> GetPropertyByAddress(string address);
    public Result<Property> GetPropertyByName(string name);
    public Result<IEnumerable<Property>> GetPropertyByLocation(string location);
    public Result<IEnumerable<Property>> GetProperties();
    public Result Insert(Property property);
    public Result Update(Property property);
    public Result Delete(PropertyId propertyId);
    
}