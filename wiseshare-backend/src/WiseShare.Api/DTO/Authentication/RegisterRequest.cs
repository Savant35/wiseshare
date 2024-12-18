namespace WiseShare.Api.DTO.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password
);

