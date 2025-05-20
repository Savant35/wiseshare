using FluentResults;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Application.Repository;

public interface IWalletRepository
{
    Task<Result<Wallet>> GetWalletByIdAsync(WalletId walletId); // Get wallet by wallet ID
    Task<Result<Wallet>> GetWalletByUserIdAsync(UserId userId); // Get wallet by user ID
    Task<Result> InsertAsync(Wallet wallet); // Insert a new wallet
    Task<Result> UpdateAsync(Wallet wallet); // Update wallet values
    Task<Result> SaveAsync();
}
