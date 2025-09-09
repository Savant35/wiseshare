using FluentResults;
using Moq;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Application.Services;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Xunit;

public class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
    private readonly IPropertyService _propertyService;

    public PropertyServiceTests()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _propertyService = new PropertyService(_propertyRepositoryMock.Object);
    }

    [Fact]
    public async Task Test_GetPropertyById_WhenIdIsValid()
    {
        var propertyId = PropertyId.CreateUnique();
        var property = Property.Create("apartment", "123 Elm Street", "New York", 450000, "a apartment in new york");
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByIdAsync(propertyId))
            .ReturnsAsync(Result.Ok(property));

        var result = await _propertyService.GetPropertyByIdAsync(propertyId);

        Assert.True(result.IsSuccess);
        Assert.Equal("123 Elm Street", result.Value.Address);
        Assert.Equal("New York", result.Value.Location);
    }

    [Fact]
    public async Task Test_GetPropertyById_WhenIdIsInvalid()
    {
        var propertyId = PropertyId.CreateUnique();
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByIdAsync(propertyId))
            .ReturnsAsync(Result.Fail<Property>("Property not found"));

        var result = await _propertyService.GetPropertyByIdAsync(propertyId);

        Assert.True(result.IsFailed);
        Assert.Equal("Property not found", result.Errors.First().Message);
    }

    [Fact]
    public async Task Test_GetPropertiesByLocation_WhenLocationIsValid()
    {
        var location = "New York";
        var properties = new List<Property>
        {
            Property.Create("apartment", "123 Elm Street", "New York", 450000, "a apartment in new york"),
            Property.Create("mansion",   "456 Oak Avenue", "New York", 500000, "a mansion located in manhatten")
        };
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByLocationAsync(location))
            .ReturnsAsync(Result.Ok<IEnumerable<Property>>(properties));

        var result = await _propertyService.GetPropertyByLocationAsync(location);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Contains(result.Value, p => p.Address == "123 Elm Street");
        Assert.Contains(result.Value, p => p.Address == "456 Oak Avenue");
    }

    [Fact]
    public async Task Test_GetPropertiesByLocation_WhenLocationIsInvalid()
    {
        var location = "fake location";
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByLocationAsync(location))
            .ReturnsAsync(Result.Ok<IEnumerable<Property>>(new List<Property>()));

        var result = await _propertyService.GetPropertyByLocationAsync(location);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Test_InsertProperty_WhenValid()
    {
        var property = Property.Create("apartment", "123 Elm Street", "New York", 450000, "a apartment in new york");
        _propertyRepositoryMock
            .Setup(x => x.InsertAsync(property))
            .ReturnsAsync(Result.Ok());
        _propertyRepositoryMock
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(Result.Ok());

        var result = await _propertyService.InsertAsync(property);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_UpdateProperty_WhenValid()
    {
        var property = Property.Create("apartment", "123 Elm Street", "New York", 450000, "a apartment in new york");
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByIdAsync(It.IsAny<PropertyId>()))
            .ReturnsAsync(Result.Ok(property));
        _propertyRepositoryMock
            .Setup(x => x.UpdateAsync(property))
            .ReturnsAsync(Result.Ok());
        _propertyRepositoryMock
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(Result.Ok());

        var result = await _propertyService.UpdateAsync(property);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_GetPropertyByAddress_WhenAddressIsValid()
    {
        var address = "123 Elm Street";
        var property = Property.Create("apartment", address, "New York", 450000, "a apartment in new york");
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByAddressAsync(address))
            .ReturnsAsync(Result.Ok(property));

        var result = await _propertyService.GetPropertyByAddressAsync(address);

        Assert.True(result.IsSuccess);
        Assert.Equal(address, result.Value.Address);
    }

    [Fact]
    public async Task Test_GetPropertyByAddress_WhenAddressIsInvalid()
    {
        var address = "Invalid Address";
        _propertyRepositoryMock
            .Setup(x => x.GetPropertyByAddressAsync(address))
            .ReturnsAsync(Result.Fail<Property>("Property not found"));

        var result = await _propertyService.GetPropertyByAddressAsync(address);

        Assert.True(result.IsFailed);
        Assert.Equal("Property not found", result.Errors.First().Message);
    }
}
