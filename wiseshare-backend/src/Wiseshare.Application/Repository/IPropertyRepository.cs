using FluentResults;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.Repository;
public interface IPropertyRepository{
    Task<Result<Property>> GetPropertyByIdAsync(PropertyId propertyId);
    Task<Result<Property>> GetPropertyByAddressAsync(string address);
    Task<Result<Property>> GetPropertyByNameAsync(string name);
    Task<Result<IEnumerable<Property>>> GetPropertyByLocationAsync(string location);
    Task<Result<IEnumerable<Property>>> GetPropertiesAsync();
    Task<Result> InsertAsync(Property property);
    Task<Result> UpdateAsync(Property property);
    Task<Result> DeleteAsync(PropertyId propertyId);
    Task<Result> SaveAsync();

}