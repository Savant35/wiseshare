using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wiseshare.Application.Common.Payment;
using Wiseshare.Application.Repository;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly WiseshareDbContext _dbContext;

    public PaymentRepository(WiseshareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<Payment>>> GetAllPaymentsAsync()
    {
        var payments = await _dbContext.Payments
            .OrderByDescending(p => p.CreatedDateTime)
            .ToListAsync();

        return Result.Ok(payments.AsEnumerable());
    }


    public async Task<Result<Payment>> GetPaymentByIdAsync(PaymentId paymentId)
    {
        var payment = await _dbContext.Payments
            .SingleOrDefaultAsync(p => p.Id == paymentId);

        return payment is not null
            ? Result.Ok(payment)
            : Result.Fail<Payment>("Payment not found.");
    }

    public async Task<Result<Payment>> GetPaymentByStripeIntentIdAsync(string stripePaymentIntentId)
    {
        var payment = await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId);
        if (payment is null)
            return Result.Fail<Payment>("No matching payment found for Stripe ID.");
        return Result.Ok(payment);
    }

    public async Task<Result<IEnumerable<Payment>>> GetPaymentsByUserIdAsync(UserId userId)
    {
        var payments = await _dbContext.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedDateTime)
            .AsNoTracking()
            .ToListAsync();

        return payments.Any()
            ? Result.Ok(payments.AsEnumerable())
            : Result.Fail<IEnumerable<Payment>>("No payments found");
    }


    public async Task<Result> InsertAsync(Payment payment)
    {
        try
        {
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        catch (System.Exception ex)
        {
            return Result.Fail($"Failed to insert Payment: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Payment payment)
    {
        var existing = await _dbContext.Payments
            .SingleOrDefaultAsync(p => p.Id == payment.Id);

        if (existing is null)
            return Result.Fail("Payment not found.");

        _dbContext.Payments.Update(payment);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(PaymentId paymentId)
    {
        var payment = await _dbContext.Payments
            .SingleOrDefaultAsync(p => p.Id == paymentId);

        if (payment is null)
            return Result.Fail("Payment not found for deletion.");

        _dbContext.Payments.Remove(payment);
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<Payment>>> GetFilteredPaymentsAsync(PaymentFilter filter)
    {
        var query = _dbContext.Payments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<PaymentStatus>(filter.Status, true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.Type) &&
            Enum.TryParse<PaymentType>(filter.Type, true, out var type))
        {
            query = query.Where(p => p.Type == type);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(p => p.CreatedDateTime >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(p => p.CreatedDateTime <= filter.ToDate.Value);
        }

        if (filter.UserId.HasValue)
        {
            query = query.Where(p => p.UserId.Value == filter.UserId.Value);
        }

        var payments = await query
            .OrderByDescending(p => p.CreatedDateTime)
            .AsNoTracking()
            .ToListAsync();

        return Result.Ok(payments.AsEnumerable());
    }


    public async Task<Result> SaveAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("Database error: " + ex.Message);
        }
    }
}
