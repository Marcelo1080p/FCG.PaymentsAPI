using FCG.PaymentsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.PaymentsAPI.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.OrderId).IsUnique();
            e.Property(p => p.Amount).HasPrecision(18, 2);
        });
    }
}
