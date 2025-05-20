namespace Wiseshare.Api.DTO.Admin;

public record UpdateAdminRequest(
    string? Email,
    string? Phone,
    string? Password,
    string? Role,
    string? SecurityQuestion,
    string? SecurityAnswer
);
