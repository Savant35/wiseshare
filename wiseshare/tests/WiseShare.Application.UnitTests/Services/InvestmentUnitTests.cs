using FluentResults;
using Moq;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.InvestmentServices;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

public class InvestmentServiceTests{
    private readonly Mock<IInvestmentRepository> _repoMock;
    private readonly Mock<IWalletService> _walletMock;
    private readonly Mock<IPortfolioService> _portfolioMock;
    private readonly Mock<IPropertyService> _propertyMock;
    private readonly InvestmentService _svc;

    public InvestmentServiceTests()
    {
        _repoMock      = new Mock<IInvestmentRepository>();
        _walletMock    = new Mock<IWalletService>();
        _portfolioMock = new Mock<IPortfolioService>();
        _propertyMock  = new Mock<IPropertyService>();

        _svc = new InvestmentService(
            _repoMock.Object,
            _walletMock.Object,
            _portfolioMock.Object,
            _propertyMock.Object
        );
    }

    [Fact]
    public async Task GetInvestmentsAsync_ReturnsOk()
    {
        var list = new List<Investment> {
            Investment.Create(UserId.CreateUnique(), PropertyId.CreateUnique(), PortfolioId.CreateUnique(UserId.CreateUnique()), 1, 100m)
        };
        _repoMock
            .Setup(x => x.GetInvestmentsAsync())
            .Returns(Task.FromResult(Result.Ok<IEnumerable<Investment>>(list)));

        var res = await _svc.GetInvestmentsAsync();

        Assert.True(res.IsSuccess);
        Assert.Single(res.Value);
    }

    [Fact]
    public async Task GetInvestmentByPropertyIdAsync_ReturnsOk()
    {
        var pid = PropertyId.CreateUnique();
        var list = new List<Investment> {
            Investment.Create(UserId.CreateUnique(), pid, PortfolioId.CreateUnique(UserId.CreateUnique()), 2, 200m)
        };
        _repoMock
            .Setup(x => x.GetInvestmentByPropertyIdAsync(pid))
            .Returns(Task.FromResult(Result.Ok<IEnumerable<Investment>>(list)));

        var res = await _svc.GetInvestmentByPropertyIdAsync(pid);

        Assert.True(res.IsSuccess);
        Assert.Equal(pid, res.Value.First().PropertyId);
    }

    [Fact]
    public async Task GetInvestmentByUserIdAsync_ReturnsOk()
    {
        var uid = UserId.CreateUnique();
        var list = new List<Investment> {
            Investment.Create(uid, PropertyId.CreateUnique(), PortfolioId.CreateUnique(uid), 3, 300m)
        };
        _repoMock
            .Setup(x => x.GetInvestmentByUserIdAsync(uid))
            .Returns(Task.FromResult(Result.Ok<IEnumerable<Investment>>(list)));

        var res = await _svc.GetInvestmentByUserIdAsync(uid);

        Assert.True(res.IsSuccess);
        Assert.Equal(uid, res.Value.First().UserId);
    }

    [Fact]
    public async Task GetInvestmentByIdAsync_ReturnsOk()
    {
        var uid = UserId.CreateUnique();
        var pid = PropertyId.CreateUnique();
        var inv = Investment.Create(uid, pid, PortfolioId.CreateUnique(uid), 4, 400m);
        var iid = InvestmentId.CreateUnique(uid, pid);

        _repoMock
            .Setup(x => x.GetInvestmentByIdAsync(iid))
            .Returns(Task.FromResult(Result.Ok(inv)));

        var res = await _svc.GetInvestmentByIdAsync(iid);

        Assert.True(res.IsSuccess);
        Assert.Equal(uid, res.Value.UserId);
        Assert.Equal(pid, res.Value.PropertyId);
    }

    [Fact]
    public async Task GetInvestmentByIdAsync_NotFound()
    {
        var iid = InvestmentId.CreateUnique(UserId.CreateUnique(), PropertyId.CreateUnique());
        _repoMock
            .Setup(x => x.GetInvestmentByIdAsync(iid))
            .Returns(Task.FromResult(Result.Fail<Investment>("Not found")));

        var res = await _svc.GetInvestmentByIdAsync(iid);

        Assert.True(res.IsFailed);
        Assert.Equal("Not found", res.Errors.First().Message);
    }

    [Fact]
    public async Task InsertAsync_ReturnsOk()
    {
        var inv = Investment.Create(UserId.CreateUnique(), PropertyId.CreateUnique(), PortfolioId.CreateUnique(UserId.CreateUnique()), 5, 500m);
        _repoMock
            .Setup(x => x.InsertAsync(inv))
            .Returns(Task.FromResult(Result.Ok()));

        var res = await _svc.InsertAsync(inv);

        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOk()
    {
        var inv = Investment.Create(UserId.CreateUnique(), PropertyId.CreateUnique(), PortfolioId.CreateUnique(UserId.CreateUnique()), 6, 600m);
        _repoMock
            .Setup(x => x.UpdateAsync(inv))
            .Returns(Task.FromResult(Result.Ok()));

        var res = await _svc.UpdateAsync(inv);

        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsOk()
    {
        var iid = InvestmentId.CreateUnique(UserId.CreateUnique(), PropertyId.CreateUnique());
        _repoMock
            .Setup(x => x.DeleteAsync(iid))
            .Returns(Task.FromResult(Result.Ok()));

        var res = await _svc.DeleteAsync(iid);

        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_Fails()
    {
        var iid = InvestmentId.CreateUnique(UserId.CreateUnique(), PropertyId.CreateUnique());
        _repoMock
            .Setup(x => x.DeleteAsync(iid))
            .Returns(Task.FromResult(Result.Fail("Cannot delete")));

        var res = await _svc.DeleteAsync(iid);

        Assert.True(res.IsFailed);
        Assert.Equal("Cannot delete", res.Errors.First().Message);
    }

    [Fact]
    public async Task SellSharesAsync_InvalidNumber_Fails()
    {
        var res = await _svc.SellSharesAsync(UserId.CreateUnique(), PropertyId.CreateUnique(), 0);
        Assert.True(res.IsFailed);
        Assert.Contains("positive integer", res.Errors.First().Message);
    }

    [Fact]
    public async Task SellSharesAsync_InvestmentNotFound_Fails()
    {
        var uid = UserId.CreateUnique();
        var pid = PropertyId.CreateUnique();
        var iid = InvestmentId.CreateUnique(uid, pid);
        _repoMock
            .Setup(x => x.GetInvestmentByIdAsync(iid))
            .Returns(Task.FromResult(Result.Fail<Investment>("None")));

        var res = await _svc.SellSharesAsync(uid, pid, 1);

        Assert.True(res.IsFailed);
        Assert.Equal("Investment not found for this user and property.", res.Errors.First().Message);
    }
}
