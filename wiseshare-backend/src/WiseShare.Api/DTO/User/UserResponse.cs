namespace Wiseshare.Api.DTO.Users;

public record UserResponse(
    string Id,           // UserId
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Role,
    bool   IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
