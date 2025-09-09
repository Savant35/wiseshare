using FluentResults;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.PropertyServices;

public interface IPropertyService{
    Task<Result<Property>> GetPropertyByIdAsync(PropertyId propertyId);
        Task<Result<Property>> GetPropertyByNameAsync(string name);
        Task<Result<Property>> GetPropertyByAddressAsync(string address);
        Task<Result<IEnumerable<Property>>> GetPropertyByLocationAsync(string location);
        Task<Result<IEnumerable<Property>>> GetPropertiesAsync();
        Task<Result> UpdateImageUrlsAsync(PropertyId propertyId, string imageUrl);
        Task<Result> SetInvestmentsStatusAsync(PropertyId propertyId, bool investmentsEnabled);
        Task<Result> InsertAsync(Property property);
        Task<Result> DeleteAsync(PropertyId propertyId);
        Task<Result> UpdateAsync(Property property);
        Task<Result> SaveAsync();
    
}