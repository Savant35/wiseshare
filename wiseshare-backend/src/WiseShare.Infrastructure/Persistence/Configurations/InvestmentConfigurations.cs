using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.InvestmentAggregate.ValueObject;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace WiseShare.Infrastructure.Persistence.Configurations;
public class InvestmentConfigurations : IEntityTypeConfiguration<Investment>
{
    public void Configure(EntityTypeBuilder<Investment> builder)
    {
        ConfigureInvestmentTable(builder);
    }

    private void ConfigureInvestmentTable(EntityTypeBuilder<Investment> builder)
    {
        // Map entity to table
        builder.ToTable("Investments");

        // Configure primary key
        builder.HasKey(i => i.Id);

        // Configure InvestmentId (ValueObject) conversion
        builder.Property(i => i.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => InvestmentId.Create(value));


        // Configure UserId (ValueObject) conversion
        builder.Property(i => i.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));

        // Configure PropertyId (ValueObject) conversion
        builder.Property(i => i.PropertyId)
            .HasConversion(
            id => id.Value,
            value => PropertyId.Create(value));   // Convert string back to PropertyId
        builder.Property(i => i.InvestmentAmount)
            .IsRequired()
            .HasDefaultValue(0) // Ensure the database defaults to 0
            .HasPrecision(18, 2);

        builder.Property(i => i.DivedendEarned)
            .IsRequired()
            .HasDefaultValue(0f) // Ensure the database defaults to 0
            .HasPrecision(18, 2);

        builder.Property(w => w.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Property: UpdatedDateTime
        builder.Property(w => w.UpdatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired();


    }
}

