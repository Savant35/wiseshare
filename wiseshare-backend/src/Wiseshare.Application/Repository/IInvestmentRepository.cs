using FluentResults;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;
namespace Wiseshare.Application.Repository;

public interface IInvestmentRepository
{

    public Result<Investment> GetInvestmentById(InvestmentId investmentId);
    public Result<IEnumerable<Investment>> GetInvestmentByUserId(UserId userId);
    public Result<IEnumerable<Investment>> GetInvestmentByPropertyId(PropertyId propertyId);
    public Result<IEnumerable<Investment>> GetInvestments();
    public Result Insert(Investment investment);
    public Result Update(Investment investment);
}