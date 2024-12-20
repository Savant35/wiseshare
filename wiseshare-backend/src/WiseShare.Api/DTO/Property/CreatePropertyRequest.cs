namespace WiseShare.Api.DTO.Property;

public record CreatePropertyRequest(
    string Name,
    string Address,
    string Location,
    double OriginalValue,
    string Description
);
