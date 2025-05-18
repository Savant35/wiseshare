using EntityFramework.Exceptions.Common;
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
        if (property.OriginalValue < 0) return Result.Fail("Property Value cannot be negative");
        try{
        return _propertyRepository.Insert(property);
        } 
         catch (UniqueConstraintException e)
        {
            var message = e.InnerException?.Message ?? e.Message;
            //Console.WriteLine(message);

            if (message.Contains("Address"))
            {
                return Result.Fail("A Property Has already been listed at that address");
            }
            else if (message.Contains("Name"))
            {
                return Result.Fail("Name for Property is alreayd in use ");
            }
            return Result.Fail("Property Registration failed DB Error");
        }
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
