using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Repositories;
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly WiseshareDbContext _dbContext;

        public InvestmentRepository(WiseshareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Investment>> GetInvestmentByIdAsync(InvestmentId investmentId)
        {
            var investment = await _dbContext.Investments.SingleOrDefaultAsync(i => i.Id == investmentId);
            return investment is not null ? Result.Ok(investment) : Result.Fail<Investment>("Investment not found");
        }

        public async Task<Result<IEnumerable<Investment>>> GetInvestmentByPropertyIdAsync(PropertyId propertyId)
        {
            var investments = await _dbContext.Investments
                .Where(i => i.PropertyId == propertyId)
                .ToListAsync();
            return investments.Any() ? Result.Ok(investments.AsEnumerable()) : Result.Fail<IEnumerable<Investment>>("No investments found");
        }

        public async Task<Result<IEnumerable<Investment>>> GetInvestmentByUserIdAsync(UserId userId)
        {
            var investments = await _dbContext.Investments
                .Where(i => i.UserId == userId)
                .ToListAsync();
            return investments.Any()
                ? Result.Ok(investments.AsEnumerable())
                : Result.Fail<IEnumerable<Investment>>("No investments found");
        }

        public async Task<Result<IEnumerable<Investment>>> GetInvestmentsAsync()
        {
            var investments = await _dbContext.Investments.ToListAsync();
            return Result.Ok(investments.AsEnumerable());
        }

        public async Task<Result> InsertAsync(Investment investment)
        {
            await _dbContext.Investments.AddAsync(investment);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Investment investment)
        {
            _dbContext.Update(investment);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> DeleteAsync(InvestmentId investmentId)
        {
            var investment = await _dbContext.Investments.SingleOrDefaultAsync(i => i.Id == investmentId);
            if (investment is null)
                return Result.Fail("Investment not found");
            _dbContext.Investments.Remove(investment);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result> SaveAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail("Database error: " + ex.Message);
            }
        }
    }
