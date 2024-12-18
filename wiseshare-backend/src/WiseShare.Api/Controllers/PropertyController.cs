using Microsoft.AspNetCore.Mvc;
using FluentResults;
using WiseShare.Api.DTO.Property;
using Wiseshare.Application.Services;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace WiseShare.Api.Controllers;

/// Controller for managing properties.
[Route("api/property")]
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// Create a new property.
    [HttpPost("create")]
    public IActionResult CreateProperty([FromBody] CreatePropertyRequest request)
    {
        var property = Property.Create(
            name: request.Name,
            address: request.Address,
            location: request.Location,
            originalValue: request.OriginalValue,
            currentValue: request.OriginalValue,
            sharePrice: request.SharePrice,
            availableShares: request.AvailableShares,
            description: request.Description);

        var result = _propertyService.Insert(property);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(new { Message = "Property created successfully", PropertyId = property.Id.Value.ToString() });
    }

    /// Search property by ID.
    [HttpGet("search/id/{id}")]
    public IActionResult GetPropertyById(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return BadRequest("Invalid PropertyId format.");
        }

        var propertyId = PropertyId.Create(guid);
        var result = _propertyService.GetPropertyById(propertyId);

        if (result.IsFailed || result.Value == null)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(new PropertyResponse(
            Id: result.Value.Id.Value.ToString(),
            Name: result.Value.Name, 
            Address: result.Value.Address,
            Location: result.Value.Location,
            OriginalValue: result.Value.OriginalValue,
            CurrentValue: result.Value.CurrentValue,
            SharePrice: result.Value.SharePrice,
            AvailableShares: result.Value.AvailableShares,
            Description: result.Value.Description));
    }

    /// Search property by address.
    [HttpGet("search/address/{address}")]
    public IActionResult GetPropertyByAddress(string address)
    {
        var result = _propertyService.GetPropertyByAddress(address);

        if (result.IsFailed || result.Value == null)
        {
            return NotFound(result.Errors.Select(e => e.Message) ?? new[] { "Property not found by address." });
        }

        return Ok(new PropertyResponse(
            Id: result.Value.Id.Value.ToString(),
            Name: result.Value.Name,
            Address: result.Value.Address,
            Location: result.Value.Location,
            OriginalValue: result.Value.OriginalValue,
            CurrentValue: result.Value.CurrentValue,
            SharePrice: result.Value.SharePrice,
            AvailableShares: result.Value.AvailableShares,
            Description: result.Value.Description));
    }

    [HttpGet("search/name/{name}")]
    public IActionResult GetPropertyByName(string name)
    {
        var result = _propertyService.GetPropertyByName(name);

        if (result.IsFailed || result.Value == null)
        {
            return NotFound(result.Errors.Select(e => e.Message) ?? new[] { "Property not found by name." });
        }

        return Ok(new PropertyResponse(
            Id: result.Value.Id.Value.ToString(),
            Name: result.Value.Name,
            Address: result.Value.Address,
            Location: result.Value.Location,
            OriginalValue: result.Value.OriginalValue,
            CurrentValue: result.Value.CurrentValue,
            SharePrice: result.Value.SharePrice,
            AvailableShares: result.Value.AvailableShares,
            Description: result.Value.Description));
    }

    /// Search properties by location.
    [HttpGet("search/location/{location}")]
    public IActionResult GetPropertiesByLocation(string location)
    {
        var result = _propertyService.GetPropertyByLocation(location);

        if (result.IsFailed || result.Value == null)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value.Select(property => new PropertyResponse(
            Id: property.Id.Value.ToString(),
            Name: property.Name, 
            Address: property.Address,
            Location: property.Location,
            OriginalValue: property.OriginalValue,
            CurrentValue: property.CurrentValue,
            SharePrice: property.SharePrice,
            AvailableShares: property.AvailableShares,
            Description: property.Description))); 
    }
}
