using Moq;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.InvestmentServices;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

public class InvestmentUnitTests
{
    private readonly Mock<IInvestmentRepository> _investmentRepoMock;
    private IInvestmentService _investmentService;

    public InvestmentUnitTests()
    {
        _investmentRepoMock= new Mock<IInvestmentRepository>();
        _investmentService= new InvestmentService(_investmentRepoMock.Object);
    }
[Fact]
public void WhenGetById_Succeeds()
{
    // Given
    var userId = UserId.CreateUnique();
    var propertyId = PropertyId.CreateUnique();
    var investment = Investment.Create(userId,propertyId,20);
    var investmentId = InvestmentId.Create(userId,propertyId);
    _investmentRepoMock.Setup(i => i.GetInvestmentById(investmentId))
        .Returns(investment); 
    // When
    var result = _investmentService.GetInvestmentById(investmentId);
    // Then
    Assert.True(result.IsSuccess);
    Assert.Equal(propertyId, result.Value.PropertyId);
    Assert.Equal(userId, result.Value.UserId);
}
}