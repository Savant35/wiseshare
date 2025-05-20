using FluentResults;
using Wiseshare.Application.Common.Payment;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.Services{
    public interface IPaymentService{
        Task<Result<IEnumerable<Payment>>> GetAllPaymentsAsync();
        Task<Result<IEnumerable<Payment>>> GetFilteredPaymentsAsync(PaymentFilter filter);

        Task<Result<IEnumerable<Payment>>> GetPaymentsByUserAsync(UserId userId);
        Task<Result<Payment>>  GetPaymentByIdAsync(PaymentId paymentId);
       Task<Result<DepositResult>> DepositAsync(UserId userId, decimal amount);
        Task<Result<Payment>> InvestAsync(UserId userId, PropertyId propertyId, int numberOfShares);
        Task<Result> UpdateDepositStatusAsync(string stripePaymentIntentId, bool success);
       Task<Result<Payment>> WithdrawAsync(UserId userId, decimal amount);
        Task<Result<Payment>> RefundAsync(UserId userId, decimal amount, string paymentIntentId);
        Task<Result> SaveAsync();
    }
}
