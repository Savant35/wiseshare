using System.ComponentModel.DataAnnotations;

namespace Wiseshare.Api.DTO.Authentication;

public record LoginRequest(
    [Required(ErrorMessage = "Email is required")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    string Password
);
