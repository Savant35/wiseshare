using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using wiseshare.Domain.PortfolioAggregate.ValueObjects;
using Wiseshare.domain.PortfolioAggregate;  // adjust namespace if needed
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace Wiseshare.Infrastructure.Persistence.Configurations
{
    public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder.ToTable("Portfolios");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, value => PortfolioId.Create(value));

            builder.Property(p => p.UserId)
                .IsRequired()
                .HasConversion(id => id.Value, value => UserId.Create(value))
                .HasColumnType("TEXT");  // ensure type is TEXT

            // Declare UserId as an alternate key (creates a UNIQUE constraint)
            builder.HasAlternateKey(p => p.UserId);

            builder.Property(p => p.TotalInvestmentAmount)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);
            
            builder.Property(p => p.RealizedProfit)                   
                .IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(p => p.CreatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(p => p.UpdatedDateTime)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .IsRequired();
            
            // Configure the one-to-many relationship based on UserId.
            builder.HasMany(p => p.Investment)
                   .WithOne() 
                   .HasForeignKey(i => i.UserId) 
                   .HasPrincipalKey(p => p.UserId);
        }
    }
}
