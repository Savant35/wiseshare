using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Repositories;
public class PropertyRepository : IPropertyRepository
{
    private readonly WiseShareDbContext _dbContext;

    public PropertyRepository(WiseShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //add new property to db
    public Result Insert(Property property)
    {
        _dbContext.Properties.Add(property);
        _dbContext.SaveChanges();
        return Result.Ok();
    }


      public Result<Property> GetPropertyByAddress(string address)
    {
        var property = _dbContext.Properties.SingleOrDefault(p => p.Address == address);
        return property is not null
            ? Result.Ok(property) : Result.Fail("NO Property at this address");
    }


    public Result<IEnumerable<Property>> GetPropertyByLocation(string location)
    {
        var props = _dbContext.Properties.ToList();
        return Result.Ok(props.AsEnumerable()); 

    }

     public Result Update(Property property)
    {
        var existingproperty = _dbContext.Properties.SingleOrDefault(p => p.Id == property.Id);
        if (existingproperty is null) return Result.Fail("User not found.");

        // Manually update fields
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    public Result Delete(PropertyId propertyId)
    {
        var property = _dbContext.Properties.SingleOrDefault(p => p.Id == propertyId);
        if (property is null) return Result.Fail("property not found");

        _dbContext.Properties.Remove(property);
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    public Result<IEnumerable<Property>> GetProperties()
    {
        var props = _dbContext.Properties.ToList();
        return Result.Ok(props.AsEnumerable());
    }

    public Result<Property> GetPropertyById(PropertyId propertyId)
    {
        var property = _dbContext.Properties.SingleOrDefault(p => p.Id == propertyId);
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found");

    }

    public Result<Property> GetPropertyByName(string name)
    {
        var property = _dbContext.Properties.SingleOrDefault(p => p.Name == name);
        return property is not null
            ? Result.Ok(property) : Result.Fail<Property>("Property not found");
    }

    // Save changes to the database
    public Result Save()
    {
        _dbContext.SaveChanges();
        return Result.Ok();
    }
}