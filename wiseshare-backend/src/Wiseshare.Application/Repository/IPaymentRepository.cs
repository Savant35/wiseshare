using FluentResults;
using Wiseshare.Application.Common.Payment;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Application.Repository;

public interface IPaymentRepository
{
    Task<Result<IEnumerable<Payment>>> GetAllPaymentsAsync();
    Task<Result<IEnumerable<Payment>>> GetFilteredPaymentsAsync(PaymentFilter filter);

    Task<Result<Payment>> GetPaymentByIdAsync(PaymentId paymentId);
    Task<Result<Payment>> GetPaymentByStripeIntentIdAsync(string stripePaymentIntentId);
    Task<Result<IEnumerable<Payment>>> GetPaymentsByUserIdAsync(UserId userId);
    Task<Result> InsertAsync(Payment payment);
    Task<Result> UpdateAsync(Payment payment);
    Task<Result> DeleteAsync(PaymentId paymentId);
    Task<Result> SaveAsync();
}
