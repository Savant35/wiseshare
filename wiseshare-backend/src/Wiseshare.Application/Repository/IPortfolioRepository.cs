using FluentResults;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.Repository;
public interface IPortfolioRepository{
    Task<Result<Portfolio>> GetPortfolioByIdAsync(PortfolioId portfolioId);
    Task<Result<Portfolio>> GetPortfolioByUserIdAsync(UserId userId);
    Task<Result> InsertAsync(Portfolio portfolio);
    Task<Result> UpdateAsync(Portfolio portfolio);
    Task<Result> SaveAsync();

}