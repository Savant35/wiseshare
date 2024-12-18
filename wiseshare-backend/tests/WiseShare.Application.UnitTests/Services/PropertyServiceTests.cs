using FluentResults; 
using Moq; 
using Wiseshare.Application.Repository;
using Wiseshare.Application.Services;
using Wiseshare.Domain.PropertyAggregate; 
using Wiseshare.Domain.PropertyAggregate.ValueObjects; 

/// Unit tests for the PropertyService class.
public class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock; 
    private readonly IPropertyService _propertyService; 

    /// Constructor for setting up the PropertyService with a mocked repository.
    public PropertyServiceTests()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _propertyService = new PropertyService(_propertyRepositoryMock.Object);
    }

    /// Test retrieving a property by ID when the ID is valid.
    [Fact]
    public void Test_GetPropertyById_WhenIdIsValid()
    {
        // Arrange
        var propertyId = PropertyId.CreateUnique();
        var property = Property.Create("123 Elm Street", "New York", 450000, 1000, 300);
        _propertyRepositoryMock.Setup(x => x.GetPropertyById(propertyId))
            .Returns(Result.Ok(property));

        // Act
        var result = _propertyService.GetPropertyById(propertyId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("123 Elm Street", result.Value.Address);
        Assert.Equal("New York", result.Value.Location);
    }

    /// Test retrieving a property by ID when the ID is invalid.
    [Fact]
    public void Test_GetPropertyById_WhenIdIsInvalid()
    {
        // Arrange
        var propertyId = PropertyId.CreateUnique();
        _propertyRepositoryMock.Setup(x => x.GetPropertyById(propertyId))
        .Returns(Result.Fail<Property>("Property not found"));

        // Act
        var result = _propertyService.GetPropertyById(propertyId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Property not found", result.Errors.First().Message);
    }

    /// Test retrieving properties by location when the location is valid.
    [Fact]
    public void Test_GetPropertiesByLocation_WhenLocationIsValid()
    {
        // Arrange
        var location = "New York";
        var properties = new List<Property>
        {
            Property.Create("123 Elm Street", "New York", 450000, 1000, 300),
            Property.Create("456 Oak Avenue", "New York", 500000, 1200, 250)
        };
        _propertyRepositoryMock.Setup(x => x.GetPropertyByLocation(location))
            .Returns(properties);

        // Act
        var result = _propertyService.GetPropertyByLocation(location);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Contains(result.Value, p => p.Address == "123 Elm Street");
        Assert.Contains(result.Value, p => p.Address == "456 Oak Avenue");
    }

    /// Test retrieving properties by location when the location is invalid.
    [Fact]
    public void Test_GetPropertiesByLocation_WhenLocationIsInvalid()
    {
        // Arrange
        var properties = new List<Property > { };
        var location = "fake location";
        _propertyRepositoryMock.Setup(x => x.GetPropertyByLocation(location))
       .Returns(Result.Ok<IEnumerable<Property>>(properties));

        // Act
        var result = _propertyService.GetPropertyByLocation(location);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(new List<Property>(), result.Value);
    }

    /// Test inserting a valid property.
    [Fact]
    public void Test_InsertProperty_WhenValid()
    {
        // Arrange
        var property = Property.Create("123 Elm Street", "New York", 450000, 1000, 300);
        _propertyRepositoryMock.Setup(x => x.Insert(property))
        .Returns(Result.Ok());

        // Act
        var result = _propertyService.Insert(property);

        // Assert
        Assert.True(result.IsSuccess);
    }

    /// Test updating a valid property.
    [Fact]
    public void Test_UpdateProperty_WhenValid()
    {
        // Arrange
        var property = Property.Create("123 Elm Street", "New York", 450000, 1000, 300);
        _propertyRepositoryMock.Setup(x => x.Update(property))
            .Returns(Result.Ok());

        // Act
        var result = _propertyService.Update(property);

        // Assert
        Assert.True(result.IsSuccess);
    }

    /// Test retrieving properties by address when the address is valid.
    [Fact]
    public void Test_GetPropertyByAddress_WhenAddressIsValid()
    {
        // Arrange
        var address = "123 Elm Street";
        var property = Property.Create("123 Elm Street", "New York", 450000, 1000, 300);
        _propertyRepositoryMock.Setup(x => x.GetPropertyByAddress(address))
            .Returns(Result.Ok(property));

        // Act
        var result = _propertyService.GetPropertyByAddress(address);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(address, result.Value.Address);
    }

    /// Test retrieving properties by address when the address is invalid.
    [Fact]
    public void Test_GetPropertyByAddress_WhenAddressIsInvalid()
    {
        // Arrange
        var address = "Invalid Address";
        _propertyRepositoryMock.Setup(x => x.GetPropertyByAddress(address))
            .Returns(Result.Fail<Property>("Property not found"));

        // Act
        var result = _propertyService.GetPropertyByAddress(address);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Property not found", result.Errors.First().Message);
    }
}
