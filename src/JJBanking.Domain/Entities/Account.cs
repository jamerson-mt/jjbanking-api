namespace JJBanking.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Owner { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Construtor necessário para o Entity Framework
    private Account() { }

    public Account(string owner, decimal initialDeposit)
    {
        Owner = owner;
        Balance = initialDeposit;
    }
}
