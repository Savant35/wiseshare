using EntityFramework.Exceptions.Common;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Repositories;
public class PropertyRepository : IPropertyRepository
{
    private readonly WiseshareDbContext _dbContext;

    public PropertyRepository(WiseshareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //add new property to db
   public async Task<Result> InsertAsync(Property property)
        {
            try
            {
                await _dbContext.Properties.AddAsync(property);
                return Result.Ok();
            }
            catch (UniqueConstraintException e)
            {
                var message = e.InnerException?.Message ?? e.Message;
                if (message.Contains("Name"))
                {
                    return Result.Fail("A property with the same Name already exists.");
                }
                else if (message.Contains("Address"))
                {
                    return Result.Fail("A property with the same Address already exists.");
                }
                return Result.Fail("Property creation failed due to a database error.");
            }
        }


    public async Task<Result<Property>> GetPropertyByAddressAsync(string address)
    {
        var property = await _dbContext.Properties.SingleOrDefaultAsync(p => p.Address == address);
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("No property at this address");
    }


    public async Task<Result<IEnumerable<Property>>> GetPropertyByLocationAsync(string location){
        var props = await _dbContext.Properties.ToListAsync();
        return Result.Ok(props.AsEnumerable());
    }

    public async Task<Result> UpdateAsync(Property property){

        var existingProperty = await _dbContext.Properties.SingleOrDefaultAsync(p => p.Id == property.Id);
        if (existingProperty is null)
            return Result.Fail("Property not found.");

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(PropertyId propertyId){

        var property = await _dbContext.Properties.SingleOrDefaultAsync(p => p.Id == propertyId);
        if (property is null) return Result.Fail("Property not found");

        _dbContext.Properties.Remove(property);
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<Property>>> GetPropertiesAsync(){
        var props = await _dbContext.Properties.ToListAsync();
        return Result.Ok(props.AsEnumerable());
    }

    public async Task<Result<Property>> GetPropertyByIdAsync(PropertyId propertyId)
    {
        var property = await _dbContext.Properties.SingleOrDefaultAsync(p => p.Id == propertyId);
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found");
    }

    public async Task<Result<Property>> GetPropertyByNameAsync(string name)
    {
        var property = await _dbContext.Properties.SingleOrDefaultAsync(p => p.Name == name);
        return property is not null
            ? Result.Ok(property)
            : Result.Fail<Property>("Property not found");
    }

    public async Task<Result> SaveAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (UniqueConstraintException e)
            {
                var message = e.InnerException?.Message ?? e.Message;
                if (message.Contains("Name"))
                    return Result.Fail("A property with the same Name already exists.");
                if (message.Contains("Address"))
                    return Result.Fail("A property with the same Address already exists.");
                return Result.Fail("Property save failed due to a database error.");
            }
        }
    }