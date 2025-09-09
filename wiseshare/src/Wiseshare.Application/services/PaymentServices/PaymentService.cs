using FluentResults;
using Microsoft.Extensions.Configuration;
using Stripe;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Application.Common.Payment;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.Services;
public class PaymentService : IPaymentService{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IWalletService _walletService;
    private readonly IPortfolioService _portfolioService;
    private readonly IPropertyService _propertyService;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public PaymentService(IPaymentRepository paymentRepository, IInvestmentRepository investmentRepository,
        IWalletService walletService, IPortfolioService portfolioService, IPropertyService propertyService,
        IUserService userService, IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _investmentRepository = investmentRepository;
        _walletService = walletService;
        _portfolioService = portfolioService;
        _propertyService = propertyService;
        _userService = userService;
        _configuration = configuration;
    }


    public async Task<Result<IEnumerable<Payment>>> GetAllPaymentsAsync()
    {
        return await _paymentRepository.GetAllPaymentsAsync();
    }
    public async Task<Result<IEnumerable<Payment>>> GetFilteredPaymentsAsync(PaymentFilter filter)
    {
        return await _paymentRepository.GetFilteredPaymentsAsync(filter);
    }



    public async Task<Result<IEnumerable<Payment>>> GetPaymentsByUserAsync(UserId userId)
    {
        return await _paymentRepository.GetPaymentsByUserIdAsync(userId);
    }

    public async Task<Result<Payment>> GetPaymentByIdAsync(PaymentId paymentId)
    {
        return await _paymentRepository.GetPaymentByIdAsync(paymentId);
    }


  public async Task<Result<DepositResult>> DepositAsync(UserId userId, decimal amount){
    if (amount <= 0)
        return Result.Fail<DepositResult>("Amount must be greater than zero.");

    // 1) ensure user has a wallet
    var walletRes = await _walletService.GetWalletByUserIdAsync(userId);
    if (walletRes.IsFailed)
        return Result.Fail<DepositResult>("Invalid User Id");

    // 2) load the actual User to get their email
    var userRes = await _userService.GetUserByIdAsync(userId);
    if (userRes.IsFailed)
        return Result.Fail<DepositResult>("User not found.");
    var user = userRes.Value;

    StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

    // 3) first-time deposit? create Connect account + onboarding link
    if (string.IsNullOrEmpty(user.StripeAccountId))
    {
        var acctSvc = new AccountService();
        var acct = await acctSvc.CreateAsync(new AccountCreateOptions
        {
            Type    = "express",
            Country = "US",
            Email   = user.Email,
            Capabilities = new AccountCapabilitiesOptions
            {
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                Transfers    = new AccountCapabilitiesTransfersOptions    { Requested = true }
            }
        });

        user.SetStripeAccountId(acct.Id);
        var upd = await _userService.UpdateAsync(user);
        if (upd.IsFailed)
            return Result.Fail<DepositResult>("Failed to save Stripe account.");
        await _userService.SaveAsync();

        var linkSvc = new AccountLinkService();
        var link = await linkSvc.CreateAsync(new AccountLinkCreateOptions
        {
            Account    = acct.Id,
            Type       = "account_onboarding",
            RefreshUrl = "https://wiseshareapi.aliarthur.com/stripe/refresh",
            ReturnUrl  = "https://wiseshareapi.aliarthur.com/stripe/complete"
        });

        return Result.Ok(new DepositResult
        {
            OnboardingUrl = link.Url,
            Message       = "Before you can deposit, your identity must be verified and bank details added so funds can be returned to your account."
        });
    }

    // 4) re-onboarding: check for any missing requirements
    var acctService = new AccountService();
    var stripeAcct  = await acctService.GetAsync(user.StripeAccountId!);
    if (stripeAcct.Requirements.CurrentlyDue.Any() ||
        stripeAcct.Requirements.PastDue.Any())
    {
        var linkSvc = new AccountLinkService();
        var link   = await linkSvc.CreateAsync(new AccountLinkCreateOptions
        {
            Account    = user.StripeAccountId,
            Type       = "account_onboarding",
            RefreshUrl = "https://wiseshareapi.aliarthur.com/stripe/refresh",
            ReturnUrl  = "https://wiseshareapi.aliarthur.com/stripe/complete"
        });

        return Result.Ok(new DepositResult
        {
            OnboardingUrl = link.Url,
            Message       = "Your identity or bank details are missing. Please complete onboarding."
        });
    }

    // 5) already onboarded → create PaymentIntent
    var piSvc = new PaymentIntentService();
    var pi    = await piSvc.CreateAsync(new PaymentIntentCreateOptions
    {
        Amount             = (long)(amount * 100),
        Currency           = "usd",
        PaymentMethodTypes = new List<string> { "card" }
    });

    // 6) record in Payments table
    var payment = Payment.CreateDeposit(userId, amount, pi.Id);
    var ins     = await _paymentRepository.InsertAsync(payment);
    if (ins.IsFailed)
        return Result.Fail<DepositResult>("Failed to record deposit.");
    await _paymentRepository.SaveAsync();

    return Result.Ok(new DepositResult
    {
        PaymentId    = payment.Id.Value.ToString(),
        ClientSecret = pi.ClientSecret
    });
}




    public async Task<Result<Payment>> WithdrawAsync(UserId userId, decimal amount)
{
    if (amount <= 0)
        return Result.Fail<Payment>("Amount must be greater than zero.");

    // 1) load user & ensure they have an Express account
    var userRes = await _userService.GetUserByIdAsync(userId);
    if (userRes.IsFailed)
        return Result.Fail<Payment>("User not found.");
    var user = userRes.Value;

    if (string.IsNullOrEmpty(user.StripeAccountId))
        return Result.Fail<Payment>("You must complete onboarding before withdrawing.");

    // 2) load wallet and ensure sufficient balance
    var walletRes = await _walletService.GetWalletByUserIdAsync(userId);
    if (walletRes.IsFailed)
        return Result.Fail<Payment>("Wallet not found.");
    var wallet = walletRes.Value;

    if (wallet.Balance < amount)
        return Result.Fail<Payment>("Insufficient balance.");

    try
    {
        // 3) debit your internal wallet
        wallet.UpdateBalance(wallet.Balance - amount);
        var wUpdate = await _walletService.UpdateAsync(wallet);
        if (wUpdate.IsFailed)
            return Result.Fail<Payment>("Failed to update wallet balance.");
        await _walletService.SaveAsync();

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        // 4) create a Transfer to the connected account
        var transferSvc = new TransferService();
        await transferSvc.CreateAsync(new TransferCreateOptions
        {
            Amount      = (long)(amount * 100),
            Currency    = "usd",
            Destination = user.StripeAccountId
        });

        // 5) trigger an immediate payout from the connected account
        var payoutSvc = new PayoutService();
        await payoutSvc.CreateAsync(new PayoutCreateOptions
        {
            Amount   = (long)(amount * 100),
            Currency = "usd"
        }, new RequestOptions
        {
            StripeAccount = user.StripeAccountId
        });

        // 6) record the withdrawal in your Payments table
        var payment = Payment.CreateWithdrawal(userId, amount);
        var insertRes = await _paymentRepository.InsertAsync(payment);
        if (insertRes.IsFailed)
            return Result.Fail<Payment>("Failed to record withdrawal.");
        await _paymentRepository.SaveAsync();

        return Result.Ok(payment);
    }
    catch (Exception ex)
    {
        // 7) rollback: refund the DB balance
        wallet.UpdateBalance(wallet.Balance + amount);
        await _walletService.UpdateAsync(wallet);
        await _walletService.SaveAsync();

        return Result.Fail<Payment>($"Withdrawal failed: {ex.Message}");
    }
}



    public async Task<Result<Payment>> RefundAsync(UserId userId, decimal amount, string paymentIntentId)
    {
        if (amount <= 0)
            return Result.Fail<Payment>("Amount must be greater than zero.");

        var walletResult = await _walletService.GetWalletByUserIdAsync(userId);
        if (walletResult.IsFailed)
            return Result.Fail<Payment>("Wallet not found.");
        var wallet = walletResult.Value;

        if (wallet.Balance < amount)
            return Result.Fail<Payment>("Insufficient balance.");

        try
        {
            // 1) call Stripe first
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = (long)(amount * 100)
            };
            var stripeRefund = await new RefundService().CreateAsync(refundOptions);

            // 2) only now debit the wallet
            wallet.UpdateBalance(wallet.Balance - amount);
            var walletUpdateResult = await _walletService.UpdateAsync(wallet);
            if (walletUpdateResult.IsFailed)
                return Result.Fail<Payment>("Failed to update wallet.");

            // 3) record the refund
            var payment = Payment.CreateRefund(userId, amount, stripeRefund.Id);
            var insertResult = await _paymentRepository.InsertAsync(payment);
            if (insertResult.IsFailed)
                return Result.Fail<Payment>("Failed to save refund record.");
            await _paymentRepository.SaveAsync();

            return Result.Ok(payment);
        }
        catch (StripeException ex)
        {
            // Stripe failed—nothing has been saved, so no rollback needed
            return Result.Fail<Payment>($"Stripe error: {ex.Message}");
        }
    }


    public async Task<Result> UpdateDepositStatusAsync(string stripePaymentIntentId, bool success)
    {
        var paymentRes = await _paymentRepository.GetPaymentByStripeIntentIdAsync(stripePaymentIntentId);
        if (paymentRes.IsFailed) return Result.Fail("Payment record not found");

        var payment = paymentRes.Value;
        if (payment.Status != PaymentStatus.Pending) return Result.Fail("Already updated");

        if (success)
        {
            payment.MarkAsCompleted();
            var walletRes = await _walletService.GetWalletByUserIdAsync(payment.UserId);
            if (walletRes.IsFailed) return Result.Fail("Wallet not found");
            var wallet = walletRes.Value;
            wallet.UpdateBalance(wallet.Balance + payment.Amount);
            await _walletService.UpdateAsync(wallet);
        }
        else
        {
            payment.MarkAsFailed();
        }

        await _paymentRepository.UpdateAsync(payment);
        await _paymentRepository.SaveAsync();
        return Result.Ok();
    }



    public async Task<Result<Payment>> InvestAsync(UserId userId, PropertyId propertyId, int numberOfShares)
    {
        if (numberOfShares <= 0)
            return Result.Fail<Payment>("Number of shares must be a positive integer.");

        var propertyResult = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (propertyResult.IsFailed)
            return Result.Fail<Payment>("Property not found.");
        var property = propertyResult.Value;

        if (!property.InvestmentsEnabled)
            return Result.Fail<Payment>("Investments for this property are currently not being accepted.");

        decimal totalCost = (decimal)property.SharePrice * numberOfShares;

        if (property.AvailableShares < numberOfShares)
            return Result.Fail<Payment>("Not enough shares remain in the property.");

        var walletResult = await _walletService.GetWalletByUserIdAsync(userId);
        if (walletResult.IsFailed)
            return Result.Fail<Payment>("Wallet not found.");
        var wallet = walletResult.Value;
        if (wallet.Balance < totalCost)
            return Result.Fail<Payment>("Insufficient balance.");

        wallet.UpdateBalance(wallet.Balance - totalCost);
        var walletUpdateResult = await _walletService.UpdateAsync(wallet);
        if (walletUpdateResult.IsFailed)
            return Result.Fail<Payment>("Failed to update wallet after investment.");

        var portfolioResult = await _portfolioService.GetPortfolioByUserIdAsync(userId);
        if (portfolioResult.IsFailed)
            return Result.Fail<Payment>("User portfolio not found.");
        var portfolio = portfolioResult.Value;
        var realPortfolioId = PortfolioId.Create(portfolio.Id.Value);

        var investmentId = InvestmentId.CreateUnique(userId, propertyId);
        Investment investment;
        var existingInvestmentResult = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
        if (existingInvestmentResult.IsSuccess && existingInvestmentResult.Value is not null)
        {
            investment = existingInvestmentResult.Value;
            investment.AddShares(numberOfShares, totalCost);

            var updateInvResult = await _investmentRepository.UpdateAsync(investment);
            if (updateInvResult.IsFailed)
                return Result.Fail<Payment>("Failed to update existing investment record.");
        }
        else
        {
            investment = Investment.Create(userId, propertyId, realPortfolioId, numberOfShares, totalCost);
            var insertInvResult = await _investmentRepository.InsertAsync(investment);
            if (insertInvResult.IsFailed)
                return Result.Fail<Payment>("Failed to create new investment record.");
        }

        portfolio.IncreaseTotalInvestmentAmount(totalCost);
        var portfolioUpdateResult = await _portfolioService.UpdateAsync(portfolio);
        if (portfolioUpdateResult.IsFailed)
            return Result.Fail<Payment>("Failed to update portfolio with new investment.");

        property.UpdateAvailableShares(property.AvailableShares - numberOfShares);
        var updatePropertyResult = await _propertyService.UpdateAsync(property);
        if (updatePropertyResult.IsFailed)
            return Result.Fail<Payment>("Failed to update property's available shares.");

        var payment = Payment.CreateInvestment(userId, totalCost);
        var paymentInsertResult = await _paymentRepository.InsertAsync(payment);
        if (paymentInsertResult.IsFailed)
            return Result.Fail<Payment>("Failed to create Payment record for the investment.");

        await _paymentRepository.SaveAsync();
        return Result.Ok(payment);
    }
    public async Task<Result> SaveAsync()
    {
        return await _paymentRepository.SaveAsync();
    }



}
