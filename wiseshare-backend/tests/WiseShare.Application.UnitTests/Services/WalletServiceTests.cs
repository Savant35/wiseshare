using FluentResults;
using Moq;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;
using Wiseshare.Application.Services;
using Wiseshare.Application.Repository;
using Xunit;

public class WalletServiceTests
{
    private readonly Mock<IWalletRepository> _walletRepositoryMock = new Mock<IWalletRepository>();
    private readonly IWalletService _walletService;

    public WalletServiceTests()
    {
        _walletService = new WalletService(_walletRepositoryMock.Object);
    }

    [Fact]
    public async Task Test_GetWalletById_WhenWalletIdIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var walletId = WalletId.CreateUnique(userId);
        var wallet = Wallet.Create(userId);
        _walletRepositoryMock
            .Setup(x => x.GetWalletByIdAsync(walletId))
            .ReturnsAsync(Result.Ok(wallet));

        // Act
        var result = await _walletService.GetWalletByIdAsync(walletId);

        // Assert
        Assert.True(result.IsSuccess, "Expected result to be successful.");
        Assert.NotNull(result.Value);
        Assert.Equal(userId,      result.Value.UserId);
        Assert.Equal(0,           result.Value.Balance);
        Assert.Equal(wallet.CreatedDateTime, result.Value.CreatedDateTime);
        Assert.Equal(wallet.UpdatedDateTime, result.Value.UpdatedDateTime);
    }

    [Fact]
    public async Task Test_GetWalletById_WhenWalletIdIsInvalid()
    {
        // Arrange
        var walletId = WalletId.Create("Invalid_Wallet_Id");
        _walletRepositoryMock
            .Setup(x => x.GetWalletByIdAsync(walletId))
            .ReturnsAsync(Result.Fail<Wallet>("Wallet not found."));

        // Act
        var result = await _walletService.GetWalletByIdAsync(walletId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Wallet not found.", result.Errors.First().Message);
    }

    [Fact]
    public async Task Test_GetWalletByUserId_WhenUserIdIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId);
        _walletRepositoryMock
            .Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(Result.Ok(wallet));

        // Act
        var result = await _walletService.GetWalletByUserIdAsync(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(0,      result.Value.Balance);
        Assert.Equal(wallet.CreatedDateTime, result.Value.CreatedDateTime);
        Assert.Equal(wallet.UpdatedDateTime, result.Value.UpdatedDateTime);
    }

    [Fact]
    public async Task Test_GetWalletByUserId_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        _walletRepositoryMock
            .Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(Result.Fail<Wallet>("Wallet not found."));

        // Act
        var result = await _walletService.GetWalletByUserIdAsync(userId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Wallet not found.", result.Errors.First().Message);
    }

    [Fact]
    public async Task Test_Update_WhenWalletIsValid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId);
        _walletRepositoryMock
            .Setup(x => x.UpdateAsync(wallet))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _walletService.UpdateAsync(wallet);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_Update_WhenWalletIsInvalid()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var wallet = Wallet.Create(userId);
        _walletRepositoryMock
            .Setup(x => x.UpdateAsync(wallet))
            .ReturnsAsync(Result.Fail("Failed to update wallet."));

        // Act
        var result = await _walletService.UpdateAsync(wallet);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Failed to update wallet.", result.Errors.First().Message);
    }
}
