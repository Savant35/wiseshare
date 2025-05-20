namespace Wiseshare.Api.DTO.Users;

public record SearchUserRequest(
    string? Id = null,     
    string? Email = null,  
    string? Phone = null   
);
