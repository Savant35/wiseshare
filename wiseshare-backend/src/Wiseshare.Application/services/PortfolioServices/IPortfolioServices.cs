using FluentResults;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.services.PortfolioServices;
public interface IPortfolioService {

public Result<Portfolio> GetPortfolioById(PortfolioId portfolioId);
public Result<Portfolio> GetPortfolioByUserId(UserId userId);

public Result Insert(Portfolio portfolio);
public Result Update(Portfolio portfolio);
}