using FluentResults;
using WiseShare.Application.Repository;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public Result<Wallet> GetWalletById(WalletId walletId)
    {
        return _walletRepository.GetWalletById(walletId);
    }

    public Result<Wallet> GetWalletByUserId(UserId userId)
    {
        return _walletRepository.GetWalletByUserId(userId);
    }

    public Result Update(Wallet wallet)
    {
        return _walletRepository.Update(wallet);
    }
}
