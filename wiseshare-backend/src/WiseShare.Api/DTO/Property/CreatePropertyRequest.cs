namespace WiseShare.Api.DTO.Property;

public record CreatePropertyRequest(
    string Name,
    string Address,
    string Location,
    double OriginalValue,
    double CurrentValue,
    double SharePrice,
    int AvailableShares,
    string Description
);
