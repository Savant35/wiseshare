using Microsoft.EntityFrameworkCore;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Application.Repository;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using FluentResults;

namespace Wiseshare.Infrastructure.Persistence.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly WiseshareDbContext _dbContext;

        public PortfolioRepository(WiseshareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Portfolio>> GetPortfolioByIdAsync(PortfolioId portfolioId)
        {
            var portfolio = await _dbContext.Portfolios
                .Include(p => p.Investment)
                .SingleOrDefaultAsync(p => p.Id == portfolioId);
            return portfolio is not null ? Result.Ok(portfolio) : Result.Fail<Portfolio>("Portfolio not found");
        }

        public async Task<Result<Portfolio>> GetPortfolioByUserIdAsync(UserId userId)
        {
            var portfolio = await _dbContext.Portfolios
                .Include(p => p.Investment)
                .SingleOrDefaultAsync(w => w.UserId == userId);

            return portfolio is not null ? Result.Ok(portfolio) : Result.Fail<Portfolio>("Portfolio not found");
        }

        public async Task<Result> InsertAsync(Portfolio portfolio)
        {
            await _dbContext.Portfolios.AddAsync(portfolio);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Portfolio portfolio)
        {
            _dbContext.Portfolios.Update(portfolio);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
