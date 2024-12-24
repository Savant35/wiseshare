using FluentResults;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.InvestmentServices;
public class InvestmentService : IInvestmentService
{

    private readonly IInvestmentRepository _investmentRepository;

    public InvestmentService(IInvestmentRepository investmentRepository)
    {
        _investmentRepository = investmentRepository;
    }

    public Result<IEnumerable<Investment>> GetInvestments()
    {
        return _investmentRepository.GetInvestments();
       
    }

    public Result<IEnumerable<Investment>> GetInvestmentByPropertyId(PropertyId propertyId)
    {
     return _investmentRepository.GetInvestmentByPropertyId(propertyId);
     
    }

    public Result<IEnumerable<Investment>> GetInvestmentByUserId(UserId userId)
    {
        return _investmentRepository.GetInvestmentByUserId(userId);
    }

    public Result Insert(Investment investment)
    {
        return _investmentRepository.Insert(investment);
    }

    public Result Update(Investment investment)
    {
        return _investmentRepository.Update(investment);
    }

    Result<Investment> IInvestmentService.GetInvestmentById(InvestmentId investmentId)
    {
        return _investmentRepository.GetInvestmentById(investmentId);
    }

    
}