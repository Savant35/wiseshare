using FluentResults;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public interface IWalletService
{
    Task<Result<Wallet>> GetWalletByIdAsync(WalletId walletId);
    Task<Result<Wallet>> GetWalletByUserIdAsync(UserId userId);
    Task<Result> InsertAsync(Wallet wallet);
    Task<Result> UpdateAsync(Wallet wallet);
    Task<Result> SaveAsync();
}
