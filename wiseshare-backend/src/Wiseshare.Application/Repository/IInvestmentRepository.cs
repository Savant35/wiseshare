using FluentResults;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.Repository;

public interface IInvestmentRepository
{

    Task<Result<Investment>> GetInvestmentByIdAsync(InvestmentId investmentId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentByUserIdAsync(UserId userId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentByPropertyIdAsync(PropertyId propertyId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentsAsync();
    Task<Result> InsertAsync(Investment investment);
    Task<Result> UpdateAsync(Investment investment);
    Task<Result> DeleteAsync(InvestmentId investmentId);
     Task<Result> SaveAsync();

}