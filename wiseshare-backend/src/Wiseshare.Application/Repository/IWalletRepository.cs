using FluentResults;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace WiseShare.Application.Repository;

public interface IWalletRepository {
    public Result<Wallet> GetWalletById(WalletId walletId); // Get wallet by wallet ID
    public Result<Wallet> GetWalletByUserId(UserId userId); // Get wallet by user ID
    public Result Insert(Wallet wallet); // Insert a new wallet
    public Result Update(Wallet wallet); // Update wallet values

    //public Result Delete(WalletId walletId);
}
