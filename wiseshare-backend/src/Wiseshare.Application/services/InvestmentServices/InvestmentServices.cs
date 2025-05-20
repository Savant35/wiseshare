using FluentResults;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.services.InvestmentServices;
public class InvestmentService : IInvestmentService{

    private readonly IInvestmentRepository _investmentRepository;

    private readonly IWalletService _walletService;
    private readonly IPortfolioService _portfolioService;
    private readonly IPropertyService _propertyService;

    public InvestmentService( IInvestmentRepository investmentRepository, IWalletService walletService,
    IPortfolioService portfolioService,IPropertyService propertyService){
        _investmentRepository = investmentRepository;
        _walletService = walletService;
        _portfolioService = portfolioService;
        _propertyService = propertyService;
    }


    public async Task<Result<IEnumerable<Investment>>> GetInvestmentsAsync(){
        return await _investmentRepository.GetInvestmentsAsync();
    }

    public async Task<Result<IEnumerable<Investment>>> GetInvestmentByPropertyIdAsync(PropertyId propertyId){
        return await _investmentRepository.GetInvestmentByPropertyIdAsync(propertyId);
    }

    public async Task<Result<IEnumerable<Investment>>> GetInvestmentByUserIdAsync(UserId userId){
        return await _investmentRepository.GetInvestmentByUserIdAsync(userId);
    }

    public async Task<Result<Investment>> GetInvestmentByIdAsync(InvestmentId investmentId){
        return await _investmentRepository.GetInvestmentByIdAsync(investmentId);
    }

    public async Task<Result> InsertAsync(Investment investment){
        return await _investmentRepository.InsertAsync(investment);
    }

    public async Task<Result> UpdateAsync(Investment investment){
        return await _investmentRepository.UpdateAsync(investment);
    }

    public async Task<Result> DeleteAsync(InvestmentId investmentId){
        return await _investmentRepository.DeleteAsync(investmentId);
    }

    public async Task<Result<Investment>> SellSharesAsync(UserId userId, PropertyId propertyId, int numberOfSharesToSell)
{
    // 1) Validate
    if (numberOfSharesToSell <= 0)
        return Result.Fail<Investment>("Number of shares to sell must be a positive integer.");

    // 2) Load existing investment
    var investmentId = InvestmentId.CreateUnique(userId, propertyId);
    var invResult = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
    if (invResult.IsFailed)
        return Result.Fail<Investment>("Investment not found for this user and property.");

    var investment = invResult.Value;

    // 3) Ensure they own enough shares
    int originalShares = investment.NumOfSharesPurchased;
    if (numberOfSharesToSell > originalShares)
        return Result.Fail<Investment>("Cannot sell more shares than are currently held.");

    // 4) Load the current property to get latest share price
    var propRes = await _propertyService.GetPropertyByIdAsync(propertyId);
    if (propRes.IsFailed)
        return Result.Fail<Investment>("Property not found.");

    var property = propRes.Value;
    decimal currentSharePrice = (decimal)property.SharePrice;

    // 5) Calculate financials
    decimal originalAmount = investment.InvestmentAmount;
    decimal unitCost = originalAmount / originalShares;
    decimal proceeds = currentSharePrice * numberOfSharesToSell;
    decimal costBasis = unitCost * numberOfSharesToSell;
    decimal realizedProfit = proceeds - costBasis;

    // 6) Subtract shares from investment
    investment.SubtractShares(numberOfSharesToSell);

    // 7) Update or delete the investment record
    if (investment.NumOfSharesPurchased == 0)
    {
        var deleteResult = await _investmentRepository.DeleteAsync(investmentId);
        if (deleteResult.IsFailed)
            return Result.Fail<Investment>("Failed to delete investment record after selling all shares.");
    }
    else
    {
        var updateInvResult = await _investmentRepository.UpdateAsync(investment);
        if (updateInvResult.IsFailed)
            return Result.Fail<Investment>(updateInvResult.Errors.First().Message);
    }

    // 8) Credit the user's wallet
    var walletRes = await _walletService.GetWalletByUserIdAsync(userId);
    if (walletRes.IsFailed)
        return Result.Fail<Investment>(walletRes.Errors);
    var wallet = walletRes.Value;
    wallet.UpdateBalance(wallet.Balance + proceeds);
    var wUpdate = await _walletService.UpdateAsync(wallet);
    if (wUpdate.IsFailed)
        return Result.Fail<Investment>(wUpdate.Errors.First().Message);

    // 9) Update the portfolio
    var portfolioRes = await _portfolioService.GetPortfolioByUserIdAsync(userId);
    if (portfolioRes.IsFailed)
        return Result.Fail<Investment>("Portfolio not found.");

    var portfolio = portfolioRes.Value;
    portfolio.DecreaseTotalInvestmentAmount(costBasis);   // Subtract cost basis only
    portfolio.AdjustRealizedProfit(realizedProfit);        // Add realized profit (gain or loss)

    var pUpdate = await _portfolioService.UpdateAsync(portfolio);
    if (pUpdate.IsFailed)
        return Result.Fail<Investment>(pUpdate.Errors.First().Message);

    // 10) Return shares to the property
    property.UpdateAvailableShares(property.AvailableShares + numberOfSharesToSell);
    var propUpdate = await _propertyService.UpdateAsync(property);
    if (propUpdate.IsFailed)
        return Result.Fail<Investment>(propUpdate.Errors.First().Message);

    // 11) All done
    return Result.Ok(investment);
}





    public async Task<Result> RequestSellSharesAsync(UserId userId, PropertyId propertyId, int sharesToSell)
    {
        var invId = InvestmentId.CreateUnique(userId, propertyId);
        var invResult = await _investmentRepository.GetInvestmentByIdAsync(invId);
        if (invResult.IsFailed)
            return Result.Fail("Investment not found.");

        var investment = invResult.Value;
        try
        {
            investment.RequestSell(sharesToSell);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }

        return await _investmentRepository.UpdateAsync(investment);
    }

    // 2) Admin approves the sale
    public async Task<Result> ApproveSellSharesAsync(UserId userId, PropertyId propertyId)
{
    // 1) Load the investment
    var invId     = InvestmentId.CreateUnique(userId, propertyId);
    var invResult = await _investmentRepository.GetInvestmentByIdAsync(invId);
    if (invResult.IsFailed)
        return Result.Fail("Investment not found.");

    var investment   = invResult.Value;
    var sharesToSell = investment.PendingSharesToSell;

    // 2) Clear the pending flag (ApproveSell now only does that)
    try
    {
        investment.ApproveSell();
    }
    catch (Exception ex)
    {
        return Result.Fail(ex.Message);
    }

    // 3) Persist the cleared-pending state
    var flagUpdate = await _investmentRepository.UpdateAsync(investment);
    if (flagUpdate.IsFailed)
        return flagUpdate;

    
    var saleResult = await SellSharesAsync(userId, propertyId, sharesToSell);
    if (saleResult.IsFailed)
        return Result.Fail(saleResult.Errors);

    // 5) Done
    return Result.Ok();
}


    public async Task<Result> RevaluePropertyAsync(PropertyId propertyId){
        // 1) load updated share price
        var propRes = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (propRes.IsFailed) return Result.Fail("Property not found.");
        var price = (decimal)propRes.Value.SharePrice;

        // 2) fetch & update all investments for that property
        var invRes = await _investmentRepository.GetInvestmentByPropertyIdAsync(propertyId);
        if (invRes.IsFailed) return Result.Fail("No investments found.");
        foreach (var inv in invRes.Value)
        {
            inv.RecalculateMarketValue(price);
            await _investmentRepository.UpdateAsync(inv);

        }

        // 4) commit changes
        await _investmentRepository.SaveAsync();
        //await _portfolioService.SaveAsync();
        return Result.Ok();
    }
    public async Task<Result> SaveAsync(){
            return await _investmentRepository.SaveAsync();
        }
}

