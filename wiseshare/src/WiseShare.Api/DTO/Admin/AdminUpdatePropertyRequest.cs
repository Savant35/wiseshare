
using System.ComponentModel.DataAnnotations;

namespace Wiseshare.Api.DTO.Admin;

public record AdminUpdatePropertyRequest(
    string? Name,
    string? Address,
    string? Location,

    [Range(0, double.MaxValue, ErrorMessage = "Original value must be non-negative")]
    double? OriginalValue,

    [Range(0, double.MaxValue, ErrorMessage = "Current value must be non-negative")]
    double? CurrentValue,

    [Range(0, int.MaxValue, ErrorMessage = "Available shares must be non-negative")]
    int? AvailableShares,

    string? Description,
    
    bool? InvestmentsEnabled
);
