using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.UserAggregate.ValueObjects;
using Wiseshare.Domain.WalletAggregate;
using Wiseshare.Domain.WalletAggregate.ValueObjects;

public class UserConfigurations : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        ConfigureWalletsTable(builder);
    }
    public void ConfigureWalletsTable(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => WalletId.Create(value));

        // Property: UserId
        builder.Property(w => w.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));

        // Property: Balance
        builder.Property(w => w.Balance)
            .IsRequired()
            .HasPrecision(18, 2) // Suitable for monetary values
            .HasDefaultValue(0); // Default balance is 0

        // Property: CreatedDateTime
        builder.Property(w => w.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Property: UpdatedDateTime
        builder.Property(w => w.UpdatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')")
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired();

        // Unique Constraint: One wallet per user
        builder.HasIndex(w => w.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Wallets_UserId");
    }
}