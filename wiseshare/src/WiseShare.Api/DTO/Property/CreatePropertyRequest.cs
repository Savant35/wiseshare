using System.ComponentModel.DataAnnotations;

namespace Wiseshare.Api.DTO.Property;

public record CreatePropertyRequest(
    [Required(ErrorMessage = "Property name is required")]
    string Name,

    [Required(ErrorMessage = "Address is required")]
    string Address,
    
    [Required(ErrorMessage = "Location is required")]
    string Location,
    
    [Range(0.0, double.MaxValue, ErrorMessage = "Original value must be non-negative")]
    double OriginalValue,

    [Required(ErrorMessage = "Description is required")]
    string Description
);
