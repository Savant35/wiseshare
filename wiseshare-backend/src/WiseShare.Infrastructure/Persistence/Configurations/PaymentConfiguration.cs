using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PaymentAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PaymentId.Create(value));

        builder.Property(i => i.UserId)
                .IsRequired()
                .HasConversion(id => id.Value, value => UserId.Create(value))
                .HasColumnType("TEXT");  // ensure type is TEXT

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Type)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(200);

        builder.Property(p => p.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.UpdatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired();
    }
}
