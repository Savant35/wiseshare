using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;


namespace Wiseshare.Infrastructure.Persistence.Configurations;


public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        ConfigurePropertyTable(builder);
    }

    private void ConfigurePropertyTable(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
        .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => PropertyId.Create(value));

        builder.Property(p => p.Name)
            .HasMaxLength(250)
            .IsRequired();



        builder.Property(p => p.Description)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.Address)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.Location)
            .HasMaxLength(100)
            .IsRequired();


        builder.Property(p => p.OriginalValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.CurrentValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.AvailableShares)
            .HasDefaultValue(20000)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.SharePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        // Unique Constraint on Name and Address
        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.Address)
            .IsUnique();
        builder.Property(p => p.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.ImageUrls)
            .HasMaxLength(500)
            .IsRequired();


        // Set default value for UpdatedDateTime
        builder.Property(p => p.UpdatedDateTime)
            //.HasDefaultValueSql("CURRENT_TIMESTAMP") // Automatically set to current UTC time
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.InvestmentsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

    }



}