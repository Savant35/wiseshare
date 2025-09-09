using Microsoft.EntityFrameworkCore;
using Wiseshare.domain.PortfolioAggregate;
using Wiseshare.Domain.InvestmentAggregate;
using Wiseshare.Domain.PaymentAggregate;
using Wiseshare.Domain.PropertyAggregate;
using Wiseshare.Domain.UserAggregate;
using Wiseshare.Domain.WalletAggregate;

namespace Wiseshare.Infrastructure.Persistence;

public class WiseshareDbContext : DbContext
{
    public WiseshareDbContext(DbContextOptions<WiseshareDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Property> Properties { get; set; } = null!;
    public DbSet<Wallet> Wallets{ get; set; } = null!;
    public DbSet<Portfolio> Portfolios{ get; set; } = null!;
    public DbSet<Investment> Investments{ get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations for all entities in the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WiseshareDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}