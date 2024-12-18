namespace WiseShare.Api.DTO.Property;

public record SearchPropertyRequest(
    string? Id = null, // Optional: Search by PropertyId
    string? Address = null, // Optional: Search by Address
    string? Location = null // Optional: Search by Location
);
