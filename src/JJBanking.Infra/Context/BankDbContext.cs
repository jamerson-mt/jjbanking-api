using JJBanking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JJBanking.Infra.Context;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().HasKey(a => a.Id);
        base.OnModelCreating(modelBuilder);
    }
}
