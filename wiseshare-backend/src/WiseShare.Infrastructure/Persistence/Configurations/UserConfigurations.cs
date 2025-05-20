using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.UserAggregate.ValueObjects;

namespace wiseshare.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder){
        ConfigureUsersTable(builder);
    }
    public void ConfigureUsersTable(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id) //specifies that the id property of user entity is being configured.
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(u => u.FirstName)
            .HasMaxLength(50);
        //.IsRequired();

        builder.Property(u => u.LastName)
           .HasMaxLength(50);
        //.IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasMaxLength(10);
        // .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(100);
        //.IsRequired();

        builder.Property(u => u.CreatedDateTime)
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Set default value for UpdatedDateTime
        builder.Property(u => u.UpdatedDateTime)
            //.HasDefaultValueSql("CURRENT_TIMESTAMP") // Automatically set to current UTC time
            .HasDefaultValueSql("datetime('now', 'localtime')") // Converts to local timezone
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(u => u.Role)
       .HasMaxLength(20)                              // Limit role name length
       .HasDefaultValue("User")                       // Default role value
       .IsRequired();                                 // Ensure it's always set

        builder.Property(u => u.IsActive)
        .HasMaxLength(6)
        .HasDefaultValue(true)
        .IsRequired();

        builder.Property(u => u.SecurityQuestion)
        .HasMaxLength(100)
        .IsRequired();

        builder.Property(u => u.SecurityAnswer)
            .HasMaxLength(100)
            .IsRequired();

        // Indexes for better query performance and uniqueness

        builder.HasIndex(u => u.Email)
            .IsUnique(); // Enforces unique Email

        builder.HasIndex(u => u.Phone)
            .IsUnique(); // Enforces unique Phone
    }
}