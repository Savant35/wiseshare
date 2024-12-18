namespace WiseShare.Api.DTO.Authentication;

public record LoginRequest(
    string Email,
    string Password
);
