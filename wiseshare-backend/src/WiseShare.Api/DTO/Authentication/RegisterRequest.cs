using System.ComponentModel.DataAnnotations;

namespace Wiseshare.Api.DTO.Authentication;

public record RegisterRequest(
    [Required(ErrorMessage = "First name is required.")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required.")]
    string LastName,

    [Required(ErrorMessage = "Email is required.")]
    string Email,

    string Phone,

    [Required(ErrorMessage = "Password is required.")]
    string Password,

    [Required(ErrorMessage = "Security question is required.")]
    string SecurityQuestion,

    [Required(ErrorMessage = "Security answer is required.")]
    string SecurityAnswer );

