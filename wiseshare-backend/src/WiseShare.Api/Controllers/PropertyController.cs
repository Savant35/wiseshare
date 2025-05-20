using Microsoft.AspNetCore.Mvc;
using Wiseshare.Api.DTO.Property;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;

namespace Wiseshare.Api.Controllers;

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

    // Create a new property.
    [HttpPost("create")]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request){

        var property = Property.Create(
            name: request.Name,
            address: request.Address,
            location: request.Location,
            originalValue: request.OriginalValue,
            description: request.Description);

        var result = await _propertyService.InsertAsync(property);
        if (result.IsFailed){
            return BadRequest(new{
                Message = "Property creation failed",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
        }
        return Ok(new{
            Message = "Property created successfully",
            PropertyId = property.Id.Value.ToString()
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProperties(){

        var result = await _propertyService.GetPropertiesAsync();
        if (result.IsFailed || result.Value == null || !result.Value.Any()){
            return NotFound(new{
                Message = "No properties found",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
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
            Description: property.Description,
            InvestmentsEnabled: property.InvestmentsEnabled)));
    }

    // Search property by ID.
    [HttpGet("search/id/{id}")]
    public async Task<IActionResult> GetPropertyById(string id){

        if (!Guid.TryParse(id, out var guid)){
            return BadRequest(new{
                Message = "Invalid PropertyId format",
                Errors = new[] { "The provided PropertyId is not a valid GUID." }
            });
        }

        var propertyId = PropertyId.Create(guid);
        var result = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (result.IsFailed || result.Value is null){
            return NotFound(new{
                Message = "Property not found",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
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
            Description: result.Value.Description,
            InvestmentsEnabled: result.Value.InvestmentsEnabled ));
    }

    // Search property by address.
    [HttpGet("search/address/{address}")]
    public async Task<IActionResult> GetPropertyByAddress(string address){
        var result = await _propertyService.GetPropertyByAddressAsync(address);
        if (result.IsFailed){
            return NotFound(new{
                Message = "No property at this address",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
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
            Description: result.Value.Description,
            InvestmentsEnabled: result.Value.InvestmentsEnabled ));
    }

    // Search property by name.
    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> GetPropertyByName(string name){
        var result = await _propertyService.GetPropertyByNameAsync(name);
        if (result.IsFailed){
            return NotFound(new{
                Message = "Property not found",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
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
            Description: result.Value.Description,
            InvestmentsEnabled: result.Value.InvestmentsEnabled ));
    }

    // Search properties by location.
    [HttpGet("search/location/{location}")]
    public async Task<IActionResult> GetPropertiesByLocation(string location){
        var result = await _propertyService.GetPropertyByLocationAsync(location);
        if (result.IsFailed || result.Value == null){
            return NotFound(new{
                Message = "Properties not found",
                Errors = result.Errors.Select(e => e.Message).ToList()
            });
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
            Description: property.Description,
            InvestmentsEnabled: property.InvestmentsEnabled)));
    }

    [HttpPut("UpdateProperty{id:guid}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request){
        var existingPropertyResult = await _propertyService.GetPropertyByIdAsync(PropertyId.Create(id));
        if (existingPropertyResult.IsFailed || existingPropertyResult.Value is null){
            return NotFound(new{
                Message = "Property not found",
                Errors = existingPropertyResult.Errors.Select(e => e.Message).ToList()
            });
        }

        var existingProperty = existingPropertyResult.Value;
        if (!string.IsNullOrWhiteSpace(request.Name))
            existingProperty.UpdateName(request.Name);

        if (!string.IsNullOrWhiteSpace(request.Description))
            existingProperty.UpdateDescription(request.Description);

        if (request.AvailableShares.HasValue)
            existingProperty.UpdateAvailableShares(request.AvailableShares.Value);

        var updateResult = await _propertyService.UpdateAsync(existingProperty);
        if (updateResult.IsFailed){
            return BadRequest(new{
                Message = "Property update failed",
                Errors = updateResult.Errors.Select(e => e.Message).ToList()
            });
        }
        return Ok(new { Message = "Property updated successfully." });
    }

    [HttpPost("{id}/upload-images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(Guid id, [FromForm] List<IFormFile> files){

        if (files == null || !files.Any())
            return BadRequest(new { Message = "No files provided" });

        const long maxFileSizePerFile = 10 * 1024 * 1024; // 10 MB per file
        const long maxTotalSize = 1L * 1024 * 1024 * 1024; // 1 GB total
        string[] allowedExtensions = { ".png", ".webp", ".jpg", ".jpeg" };

        long totalSize = 0;
        foreach (var file in files){
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { Message = $"File type '{extension}' is not allowed" });

            if (file.Length > maxFileSizePerFile)
                return BadRequest(new { Message = $"File '{file.FileName}' exceeds the 10 MB size limit" });

            totalSize += file.Length;
        }

        if (totalSize > maxTotalSize)
            return BadRequest(new { Message = "Total size of uploaded files exceeds the 1GB limit" });

        var propertyFolder = id.ToString();
        var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images", propertyFolder);
        if (!Directory.Exists(uploadsFolderPath))
            Directory.CreateDirectory(uploadsFolderPath);

        foreach (var file in files){
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
        return Ok(new { Message = "Images uploaded successfully" });
    }

    [HttpGet("{id}/images")]
    public IActionResult GetPropertyImages(Guid id){

        var propertyFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images", id.ToString());
        if (!Directory.Exists(propertyFolderPath)){
            return NotFound(new { Message = "Image folder not found for this property", Errors = new[] { "Folder does not exist" } });
        }
        var files = Directory.GetFiles(propertyFolderPath);
        var folderUrl = $"/images/{id}/";
        var imageUrls = files.Select(file => folderUrl + Path.GetFileName(file)).ToList();
        return Ok(new { folderUrl, imageUrls });
    }

    [HttpDelete("{id}/images")]
    public IActionResult DeleteImages(Guid id, [FromQuery] List<string> fileNames){
        if (fileNames == null || !fileNames.Any())
            return BadRequest(new { Message = "At least one file name or image URL is required" });

        var propertyFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images", id.ToString());
        if (!Directory.Exists(propertyFolderPath)){
            return NotFound(new { Message = "Image folder not found for this property", Errors = new[] { "Folder does not exist" } });
        }
        var expectedPrefix = $"/images/{id}/";
        var errors = new List<string>();
        var deletedFiles = new List<string>();

        foreach (var input in fileNames){
            string fileName = input;
            if (fileName.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase)){
                fileName = fileName.Substring(expectedPrefix.Length);
            }

            if (string.IsNullOrWhiteSpace(fileName)){
                errors.Add("Invalid file name provided.");
                continue;
            }
            var filePath = Path.Combine(propertyFolderPath, fileName);
            if (!System.IO.File.Exists(filePath)){
                errors.Add($"File not found: {fileName}");
                continue;
            }
            try{
                System.IO.File.Delete(filePath);
                deletedFiles.Add(fileName);
            }
            catch (Exception ex){
                errors.Add($"Error deleting {fileName}: {ex.Message}");
            }
        }
        if (errors.Count != 0){
            return StatusCode(207, new{
                Message = "Some files could not be deleted",
                DeletedFiles = deletedFiles,
                Errors = errors
            });
        }
        return Ok(new { Message = "Files deleted successfully", DeletedFiles = deletedFiles });
    }
}
