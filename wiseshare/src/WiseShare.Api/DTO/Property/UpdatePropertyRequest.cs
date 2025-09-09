using System.ComponentModel.DataAnnotations;

namespace Wiseshare.Api.DTO.Property;

public record UpdatePropertyRequest(
    string? Name,
    string? Description,

    [Range(0, int.MaxValue, ErrorMessage = "Available shares must be non-negative")]
    int? AvailableShares
);
