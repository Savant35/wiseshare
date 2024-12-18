using Microsoft.EntityFrameworkCore;
using Wiseshare.Domain.UserAggregate;

namespace WiseShare.Infrastructure.Persistence;

public class WiseShareDbContext : DbContext
{
    public WiseShareDbContext(DbContextOptions<WiseShareDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations for all entities in the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WiseShareDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}