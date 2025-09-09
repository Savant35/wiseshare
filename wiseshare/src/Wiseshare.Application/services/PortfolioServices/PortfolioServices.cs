using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.PortfolioServices;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
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

        public async Task<Result<Portfolio>> GetPortfolioByIdAsync(PortfolioId portfolioId)
        {
            return await _portfolioRepository.GetPortfolioByIdAsync(portfolioId);
        }

        public async Task<Result<Portfolio>> GetPortfolioByUserIdAsync(UserId userId)
        {
            return await _portfolioRepository.GetPortfolioByUserIdAsync(userId);
        }

        public async Task<Result> InsertAsync(Portfolio portfolio)
        {
            return await _portfolioRepository.InsertAsync(portfolio);
        }

        public async Task<Result> UpdateAsync(Portfolio portfolio)
        {
            return await _portfolioRepository.UpdateAsync(portfolio);
        }

        public async Task<Result> SaveAsync()
        {
            return await _portfolioRepository.SaveAsync();
        }
    }
}
