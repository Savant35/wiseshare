namespace Wiseshare.Api.DTO.Admin;

public record AdminResponse(
    string Id,           // UserId
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Role,
    bool AccountStatus,
    DateTime CreatedAt,
    DateTime UpdatedAt
);