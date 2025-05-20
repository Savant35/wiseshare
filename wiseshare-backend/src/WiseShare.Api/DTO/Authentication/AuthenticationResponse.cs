namespace Wiseshare.Api.DTO.Authentication;
public record AuthenticationResponse(
    //string Id,
    string Token,
    string FirstName,
    string LastName,
    string Role
);

