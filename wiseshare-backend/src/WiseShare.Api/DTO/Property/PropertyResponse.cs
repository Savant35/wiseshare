namespace WiseShare.Api.DTO.Property;

public record PropertyResponse(
    string Id, // PropertyId
    string Name,
    string Address,
    string Location,
    double OriginalValue,
    double CurrentValue,
    double SharePrice,
    int AvailableShares,
    string Description
    //DateTime CreatedDateTime,
    //DateTime UpdatedDateTime
);
