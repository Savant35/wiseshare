using FluentResults;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.domain.PortfolioAggregate;

using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public Result<Portfolio> GetPortfolioById(PortfolioId portfolioId)
        {
            return _portfolioRepository.GetPortfolioById(portfolioId);
        }

        public Result<Portfolio> GetPortfolioByUserId(UserId userId)
        {
            return _portfolioRepository.GetPortfolioByUserId(userId);
        }

        public Result Insert(Portfolio portfolio)
        {
            return _portfolioRepository.Insert(portfolio);
        }



        public Result Update(Portfolio portfolio)
        {
            return _portfolioRepository.Update(portfolio);
        }


    }
}
