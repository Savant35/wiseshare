namespace WiseShare.Api.DTO.Authentication;

public record ResetPasswordRequest(
    string Email,
    string SecurityAnswer,
    string NewPassword);
