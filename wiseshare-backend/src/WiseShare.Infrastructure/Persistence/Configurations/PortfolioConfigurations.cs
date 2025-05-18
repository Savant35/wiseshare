using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Configurations
{
    public class PortfolioConfigurations : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            ConfigurePortfolioTable(builder);
        }

        private void ConfigurePortfolioTable(EntityTypeBuilder<Portfolio> builder)
        {
            builder.ToTable("Portfolios");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => PortfolioId.Create(value));

            builder.Property(p => p.UserId)
                .IsRequired()
                .HasConversion(
                    id => id.Value,
                    value => UserId.Create(value));

/*
            builder.HasMany(p => p.Investment)
                .WithOne(i => i.Portfolio) // Link to the navigation property in Investment
                .HasForeignKey(i => i.PortfolioId); // Use PortfolioId as the foreign key
                */

            builder.Property(p => p.TotalInvestmentAmount)
                .IsRequired()
                .HasPrecision(18, 2) // Suitable for monetary values
                .HasDefaultValue(0); // Default balance is 0

            builder.Property(p => p.TotalReturns)
                .IsRequired()
                .HasPrecision(18, 2) // Suitable for monetary values
                .HasDefaultValue(0); // Default return is 0

            // Property: CreatedDateTime
            builder.Property(p => p.CreatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .IsRequired();

            // Property: UpdatedDateTime
            builder.Property(p => p.UpdatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired();
        }
    }
}
