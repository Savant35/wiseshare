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
            .HasConversion(id => id.Value,value => UserId.Create(value));

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
            
        /* Indexes for better query performance and uniqueness

        builder.HasIndex(u => u.Email)
            .IsUnique(); // Enforces unique Email

        builder.HasIndex(u => u.Phone)
            .IsUnique(); // Enforces unique Phone
            */
    }
}