using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Repositories;
public class WalletRepository : IWalletRepository
{

    private readonly WiseshareDbContext _dbContext;


    public WalletRepository(WiseshareDbContext dbContext){
        _dbContext = dbContext;
    }

    public async Task<Result<Wallet>> GetWalletByIdAsync(WalletId walletId){
        var wallet = await _dbContext.Wallets.SingleOrDefaultAsync(w => w.Id == walletId);
        return wallet is not null ? Result.Ok(wallet) : Result.Fail("Wallet not found");
    }

    public async Task<Result<Wallet>> GetWalletByUserIdAsync(UserId userId){
        var wallet = await _dbContext.Wallets.SingleOrDefaultAsync(w => w.UserId == userId);
        return wallet is not null ? Result.Ok(wallet) : Result.Fail("Wallet not found");
    }


    public async Task<Result> InsertAsync(Wallet wallet){
        await _dbContext.Wallets.AddAsync(wallet);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(Wallet wallet){
        _dbContext.Wallets.Update(wallet);
        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }
    public async Task<Result> SaveAsync(){
        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }
}