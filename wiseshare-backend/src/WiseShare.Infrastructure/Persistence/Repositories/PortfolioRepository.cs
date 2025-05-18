using FluentResults;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Application.Repository;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Repositories.PropertyRepository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly WiseShareDbContext _dbContext;


    public PortfolioRepository(WiseShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Result<Portfolio> GetPortfolioById(PortfolioId portfolioId)
    {
        var portfolio = _dbContext.Portfolios.SingleOrDefault(p => p.Id == portfolioId);
        return portfolio is not null
            ? Result.Ok(portfolio) : Result.Fail<Portfolio>("Portfolio not found");
    }


    public Result<Portfolio> GetPortfolioByUserId(UserId userId)
    {
        var portfolio = _dbContext.Portfolios.SingleOrDefault(p => p.Id == userId);
        return portfolio is not null ? Result.Ok(portfolio) : Result.Fail<Portfolio>("portfolio not found");
    }

    public Result Insert(Portfolio portfolio)
    {
        _dbContext.Portfolios.Add(portfolio);
        _dbContext.SaveChanges();
        return Result.Ok();
    }

    public Result Update(Portfolio portfolio)
    {
        _dbContext.Portfolios.Update(portfolio);
        _dbContext.SaveChanges();
        return Result.Ok();
    }
}