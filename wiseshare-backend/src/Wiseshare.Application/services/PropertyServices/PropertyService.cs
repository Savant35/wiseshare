using FluentResults;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public class PropertyService : IPropertyService{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository){
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<Property>> GetPropertyByIdAsync(PropertyId propertyId){
        return await _propertyRepository.GetPropertyByIdAsync(propertyId);
    }

    public async Task<Result<Property>> GetPropertyByNameAsync(string name){
        return await _propertyRepository.GetPropertyByNameAsync(name);
    }

    public async Task<Result<Property>> GetPropertyByAddressAsync(string address){
        return await _propertyRepository.GetPropertyByAddressAsync(address);
    }

    public async Task<Result<IEnumerable<Property>>> GetPropertyByLocationAsync(string location){
        return await _propertyRepository.GetPropertyByLocationAsync(location);
    }

    public async Task<Result<IEnumerable<Property>>> GetPropertiesAsync(){
        return await _propertyRepository.GetPropertiesAsync();
    }

    public async Task<Result> UpdateImageUrlsAsync(PropertyId propertyId, string imageUrl){

        var propertyResult = await _propertyRepository.GetPropertyByIdAsync(propertyId);
        if (propertyResult.IsFailed)
            return Result.Fail("Property not found.");

        var property = propertyResult.Value;
        property.UpdateImageUrls(imageUrl);

        var updateResult = await _propertyRepository.UpdateAsync(property);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors.First().Message);

        var saveResult = await _propertyRepository.SaveAsync();
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }

    public async Task<Result> InsertAsync(Property property){

        if (property.OriginalValue < 0)
            return Result.Fail("Property Value cannot be negative");

        var insertResult = await _propertyRepository.InsertAsync(property);
        if (insertResult.IsFailed)
            return Result.Fail(insertResult.Errors.First().Message);

         var saveResult = await _propertyRepository.SaveAsync();
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }



    public async Task<Result> UpdateAsync(Property updatedProperty){

        if (updatedProperty is null)
            return Result.Fail("No updated property data was provided.");

        var existingPropertyResult = await _propertyRepository.GetPropertyByIdAsync(PropertyId.Create(updatedProperty.Id.Value));
        if (existingPropertyResult.IsFailed)
            return Result.Fail("Invalid property ID.");

        var existingProperty = existingPropertyResult.Value;
        if (existingProperty is null)
            return Result.Fail("Property not found.");

        if (!string.Equals(existingProperty.Name, updatedProperty.Name, StringComparison.Ordinal))
            existingProperty.UpdateName(updatedProperty.Name);

        if (!string.Equals(existingProperty.Description, updatedProperty.Description, StringComparison.Ordinal))
            existingProperty.UpdateDescription(updatedProperty.Description);

        if (existingProperty.AvailableShares != updatedProperty.AvailableShares)
            existingProperty.UpdateAvailableShares(updatedProperty.AvailableShares);
        
        var updateResult = await _propertyRepository.UpdateAsync(existingProperty);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors.First().Message);

        var saveResult = await _propertyRepository.SaveAsync();
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors.First().Message);


        return Result.Ok();
    }



    public async Task<Result> DeleteAsync(PropertyId propertyId){

        var propertyResult = await _propertyRepository.GetPropertyByIdAsync(propertyId);
        if (propertyResult.IsFailed)
            return Result.Fail("Property not found.");

        var property = propertyResult.Value;
        if (property.AvailableShares != 20000)
            return Result.Fail("Property cannot be deleted because users are currently invested in it.");

        var deleteResult = await _propertyRepository.DeleteAsync(propertyId);
        if (deleteResult.IsFailed)
            return Result.Fail(deleteResult.Errors.First().Message);
        
        var saveResult = await _propertyRepository.SaveAsync();
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok(); 
    }

    public async Task<Result> SetInvestmentsStatusAsync(PropertyId propertyId, bool enabled){

        var propertyResult = await _propertyRepository.GetPropertyByIdAsync(propertyId);
        if (propertyResult.IsFailed)
            return Result.Fail("Property not found.");

        var property = propertyResult.Value;
        if (enabled)
            property.EnableInvestments();
        else
            property.DisableInvestments();

        var updateResult = await _propertyRepository.UpdateAsync(property);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors.First().Message);

        var saveResult = await _propertyRepository.SaveAsync();
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors.First().Message);

        return Result.Ok();
    }

    public async Task<Result> SaveAsync(){
        return await _propertyRepository.SaveAsync();
    }


}
