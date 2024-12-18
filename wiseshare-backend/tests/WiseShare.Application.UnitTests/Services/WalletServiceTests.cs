using FluentResults;
using Moq;
using WiseShare.Application.Repository;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;
using Wiseshare.Application.Services;

public class WalletServiceTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock = new Mock<IWalletRepository>();
    private IWalletService _walletService;

    public WalletServiceTests()
    {
        _walletService = new WalletService(_walletRepositoryMock.Object);
    }

    [Fact]
    public void Test_GetWalletById_WhenWalletIdIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var walletId = WalletId.CreateUnique(userId);
        var wallet = Wallet.Create(userId, DateTime.UtcNow, DateTime.UtcNow);
        _walletRepositoryMock.Setup(x => x.GetWalletById(walletId))
            .Returns(Result.Ok(wallet));

        // Act
        var result = _walletService.GetWalletById(walletId);

        // Assert
        Console.WriteLine(wallet.Id); // Should print the expected WalletId

        Assert.True(result.IsSuccess, "Expected result to be successful.");
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId); // Check UserId matches
        Assert.Equal(0, result.Value.Balance); // Check balance is zero
        Assert.Equal(wallet.CreatedDateTime, result.Value.CreatedDateTime); // Check created date
        Assert.Equal(wallet.UpdatedDateTime, result.Value.UpdatedDateTime); // Check updated date
    }
    [Fact]
    public void Test_GetWalletById_WhenWalletIdIsInvalid()
    {
        // Arrange
        var walletId = WalletId.Create("Invalid_Wallet_Id");
        _walletRepositoryMock.Setup(x => x.GetWalletById(walletId))
            .Returns(Result.Fail<Wallet>("Wallet not found."));

        // Act
        var result = _walletService.GetWalletById(walletId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Wallet not found.", result.Errors.First().Message);
    }

    [Fact]
    public void Test_GetWalletByUserId_WhenUserIdIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId, DateTime.UtcNow, DateTime.UtcNow);

        _walletRepositoryMock.Setup(x => x.GetWalletByUserId(userId))
            .Returns(Result.Ok(wallet));

        // Act
        var result = _walletService.GetWalletByUserId(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId); // UserId matches
        Assert.Equal(0, result.Value.Balance); // Balance is zero
        Assert.Equal(wallet.CreatedDateTime, result.Value.CreatedDateTime); // CreatedDateTime matches
        Assert.Equal(wallet.UpdatedDateTime, result.Value.UpdatedDateTime); // UpdatedDateTime matches
    }

    [Fact]
    public void Test_GetWalletByUserId_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        _walletRepositoryMock.Setup(x => x.GetWalletByUserId(userId))
            .Returns(Result.Fail<Wallet>("Wallet not found."));

        // Act
        var result = _walletService.GetWalletByUserId(userId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Wallet not found.", result.Errors.First().Message);
    }

    [Fact]
    public void Test_Update_WhenWalletIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId, DateTime.UtcNow, DateTime.UtcNow);

        _walletRepositoryMock.Setup(x => x.Update(wallet))
            .Returns(Result.Ok());

        // Act
        var result = _walletService.Update(wallet);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Test_Update_WhenWalletIsInvalid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId, DateTime.UtcNow, DateTime.UtcNow);

        _walletRepositoryMock.Setup(x => x.Update(wallet))
            .Returns(Result.Fail("Failed to update wallet."));

        // Act
        var result = _walletService.Update(wallet);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Failed to update wallet.", result.Errors.First().Message);
    }
}
