using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Repositories;
public class WalletRepository : IWalletRepository
{

    private readonly WiseShareDbContext _dbContext;


    public WalletRepository(WiseShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Result<Wallet> GetWalletById(WalletId walletId)
    {
        var wallet = _dbContext.Wallets.SingleOrDefault(w => w.Id == walletId);
        return wallet is not null
        ? Result.Ok(wallet) : Result.Fail("Wallet not found");

    }

    public Result<Wallet> GetWalletByUserId(UserId userId)
    {
        var wallet = _dbContext.Wallets.SingleOrDefault(w => w.Id == userId);
        return wallet is not null
        ? Result.Ok(wallet) : Result.Fail("Wallet not found");


    }

    public Result Insert(Wallet wallet)
    {
         _dbContext.Wallets.Add(wallet);
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    public Result Update(Wallet wallet)
    {
        _dbContext.Wallets.Update(wallet);
        _dbContext.SaveChanges();
        return Result.Ok();
    }
}