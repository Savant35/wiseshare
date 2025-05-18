using Microsoft.EntityFrameworkCore;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.WalletAggregate;

namespace WiseShare.Infrastructure.Persistence;

public class WiseShareDbContext : DbContext
{
    public WiseShareDbContext(DbContextOptions<WiseShareDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Wallet> Wallets{ get; set; } = null!;
    public DbSet<Portfolio> Portfolios{ get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations for all entities in the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WiseShareDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}