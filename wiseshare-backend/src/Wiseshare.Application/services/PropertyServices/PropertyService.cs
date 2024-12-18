using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public Result<Property> GetPropertyById(PropertyId propertyId)
    {
        return _propertyRepository.GetPropertyById(propertyId);
    }

    public Result<Property> GetPropertyByAddress(string address)
    {
        return _propertyRepository.GetPropertyByAddress(address);

    }
    public Result<Property> GetPropertyByName(string name)
    {
        return _propertyRepository.GetPropertyByName(name);

    }


    public Result<IEnumerable<Property>> GetProperties()
    {
        return _propertyRepository.GetProperties();
    }
    public Result<IEnumerable<Property>> GetPropertyByLocation(string location)
    {
        return _propertyRepository.GetPropertyByLocation(location);
    }

    public Result Insert(Property property)
    {
        return _propertyRepository.Insert(property);
    }

    public Result Update(Property property)
    {
        

        return _propertyRepository.Update(property);
    }

    public Result Delete(PropertyId propertyId)
    {
        return _propertyRepository.Delete(propertyId);
    }
}
