using FluentResults;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.services.PortfolioServices;
public interface IPortfolioService{
    Task<Result<Portfolio>> GetPortfolioByIdAsync(PortfolioId portfolioId);
    Task<Result<Portfolio>> GetPortfolioByUserIdAsync(UserId userId);
    Task<Result> InsertAsync(Portfolio portfolio);
    Task<Result> UpdateAsync(Portfolio portfolio);
    Task<Result> SaveAsync();
}