using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Configurations
{
    public class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> builder)
        {
            builder.ToTable("Investments");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, value => InvestmentId.Create(value));

            builder.Property(i => i.UserId)
                .IsRequired()
                .HasConversion(id => id.Value, value => UserId.Create(value))
                .HasColumnType("TEXT");  // ensure type is TEXT

            builder.Property(i => i.PropertyId)
                .IsRequired()
                .HasConversion(
                    id => id.Value,
                    value => PropertyId.Create(value));

            builder.Property(i => i.PortfolioId)
                .IsRequired()
                .HasConversion(
                    pid => pid.Value,
                    value => PortfolioId.Create(value))
                .HasColumnType("TEXT");

            builder.Property(i => i.NumOfSharesPurchased)
                .IsRequired();

            builder.Property(i => i.InvestmentAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(i => i.DivedendEarned)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(i => i.CreatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.UpdatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.IsSellPending)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.PendingSharesToSell)
                .IsRequired()
                .HasDefaultValue(0);

        }
    }
}
