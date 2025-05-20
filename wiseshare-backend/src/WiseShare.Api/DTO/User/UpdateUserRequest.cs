namespace Wiseshare.Api.DTO.Users;

public record UpdateUserRequest(
    string? Email,
    string? Phone,
    string? Password,
    string? SecurityQuestion,
    string? SecurityAnswer
);
