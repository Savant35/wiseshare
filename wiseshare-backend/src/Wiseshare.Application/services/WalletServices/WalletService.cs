using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Application.Services;

public class WalletService : IWalletService{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository){
        _walletRepository = walletRepository;
    }

    public async Task<Result<Wallet>> GetWalletByIdAsync(WalletId walletId){
        return await _walletRepository.GetWalletByIdAsync(walletId);
    }

    public async Task<Result<Wallet>> GetWalletByUserIdAsync(UserId userId){
        return await _walletRepository.GetWalletByUserIdAsync(userId);
    }

    public async Task<Result> InsertAsync(Wallet wallet){
        return await _walletRepository.InsertAsync(wallet);
    }

    public async Task<Result> UpdateAsync(Wallet wallet){
        return await _walletRepository.UpdateAsync(wallet);
    }
    public async Task<Result> SaveAsync(){
        return await _walletRepository.SaveAsync();
    }
}

