using FluentResults;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.InvestmentServices;
public interface IInvestmentService
{
    Task<Result<Investment>> GetInvestmentByIdAsync(InvestmentId investmentId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentByUserIdAsync(UserId userId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentByPropertyIdAsync(PropertyId propertyId);
    Task<Result> InsertAsync(Investment investment);
    Task<Result> UpdateAsync(Investment investment);
    Task<Result> DeleteAsync(InvestmentId investmentId);
    Task<Result<Investment>> SellSharesAsync(UserId userId, PropertyId propertyId, int numberOfSharesToSell);
    Task<Result> RequestSellSharesAsync(UserId userId, PropertyId propertyId, int sharesToSell);
    Task<Result> ApproveSellSharesAsync(UserId userId, PropertyId propertyId);
    Task<Result> RevaluePropertyAsync(PropertyId propertyId);
    Task<Result<IEnumerable<Investment>>> GetInvestmentsAsync();
    Task<Result> SaveAsync();
}