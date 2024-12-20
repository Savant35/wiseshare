using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.PropertyAggregate.ValueObjects;


namespace WiseShare.Infrastructure.Persistence.Configurations;


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
            .HasMaxLength(100);
         
        builder.Property(p => p.Description)
            .HasMaxLength(250);

        builder.Property(p => p.Address);

        builder.Property(p => p.Location);

        builder.Property(p => p.OriginalValue);

        builder.Property(p => p.CurrentValue);

        builder.Property(p => p.AvailableShares);

        builder.Property(p => p.SharePrice);

        builder.Property(p => p.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Set default value for UpdatedDateTime
        builder.Property(p => p.UpdatedDateTime)
            //.HasDefaultValueSql("CURRENT_TIMESTAMP") // Automatically set to current UTC time
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();
    }
    


}