using FluentResults;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public interface IWalletService
{
    public Result<Wallet> GetWalletById(WalletId walletId);
    public Result<Wallet> GetWalletByUserId(UserId userId);
    public Result Update(Wallet wallet);
}
