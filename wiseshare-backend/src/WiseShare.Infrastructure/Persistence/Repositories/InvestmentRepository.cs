using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly WiseShareDbContext _dbContext;

        public InvestmentRepository(WiseShareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result<Investment> GetInvestmentById(InvestmentId investmentId)
        {
            var portfolio = _dbContext.Investments.SingleOrDefault(i => i.Id == investmentId);
            return portfolio is not null
                ? Result.Ok(portfolio)
                : Result.Fail<Investment>("Portfolio not found");
        }

        public Result<IEnumerable<Investment>> GetInvestmentByPropertyId(PropertyId propertyId)
        {
            var investments = _dbContext.Investments
                .Where(i => i.PropertyId == propertyId)
                .ToList();

            return investments.Any()
                ? Result.Ok(investments.AsEnumerable())
                : Result.Fail<IEnumerable<Investment>>("No investments found");
        }

        public Result<IEnumerable<Investment>> GetInvestmentByUserId(UserId userId)
        {
            var investments = _dbContext.Investments
               .Where(i => i.UserId == userId)
               .ToList();

            return investments.Any()
                ? Result.Ok(investments.AsEnumerable())
                : Result.Fail<IEnumerable<Investment>>("No investments found");
        }

        public Result<IEnumerable<Investment>> GetInvestments()
        {
            var investments = _dbContext.Investments.ToList();
            return Result.Ok(investments.AsEnumerable());
        }

        public Result Insert(Investment investment)
        {
            _dbContext.Investments.Add(investment);
            _dbContext.SaveChanges();
            return Result.Ok();
        }

        public Result Update(Investment investment)
        {
            _dbContext.Update(investment);
            _dbContext.SaveChanges();
            return Result.Ok();
        }
    }
}
