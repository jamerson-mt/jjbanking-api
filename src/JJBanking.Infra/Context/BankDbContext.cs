using JJBanking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JJBanking.Infra.Context;

public class BankDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Transfer> Transfers => Set<Transfer>(); // Nova DbSet para Transferências

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // (USER)
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Cpf).IsUnique();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
        });

        // (ACCOUNT)
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.AccountNumber).IsUnique();
            entity.Property(a => a.Balance).HasColumnType("decimal(8,2)");

            entity
                .HasOne(a => a.User)
                .WithOne(u => u.Account)
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // (TRANSACTION) - Registro individual de extrato
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(8,2)");

            entity
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);
        });

        // (TRANSFER) - Configuração da nova entidade DDD
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(8,2)").IsRequired();
            entity.Property(t => t.Description).HasMaxLength(250);
            entity.Property(t => t.CreatedAt).IsRequired();

            // Configuração do relacionamento com a Conta de Origem
            entity
                .HasOne(t => t.OriginAccount)
                .WithMany() // Se você não criou uma lista de Transfers na Account, deixe vazio
                .HasForeignKey(t => t.OriginAccountId)
                .OnDelete(DeleteBehavior.Restrict); // Importante: Restrict para evitar erros de múltiplo cascade

            // Configuração do relacionamento com a Conta de Destino
            entity
                .HasOne(t => t.DestinationAccount)
                .WithMany()
                .HasForeignKey(t => t.DestinationAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
