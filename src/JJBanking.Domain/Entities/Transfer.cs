using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JJBanking.Domain.Entities;

[Table("Transfers")]
public class Transfer
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    // CONTA DE SAIDA DO VALOR
    [Required]
    public Guid OriginAccountId { get; private set; }

    [Required]
    public Account OriginAccount { get; private set; } = null!;

    // CONTA DE RECEBIMENTO DO VALOR
    [Required]
    public Guid DestinationAccountId { get; private set; }
    public Account DestinationAccount { get; private set; } = null!;

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    [MaxLength(255)]
    public string? Description { get; private set; } = string.Empty;

    // Construtor obrigatório para o Entity Framework
    protected Transfer() { }

    // Construtor de Domínio: Onde a mágica acontece
    public Transfer(
        Guid originAccountId,
        Guid destinationAccountId,
        decimal amount,
        string description
    )
    {
        // Regras de Negócio (Guard Clauses)
        if (originAccountId == destinationAccountId)
            throw new InvalidOperationException(
                "A conta de origem não pode ser igual à de destino."
            );

        if (amount <= 0)
            throw new ArgumentException("O valor da transferência deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(description))
            description = "Transferência bancária";

        Id = Guid.NewGuid();
        OriginAccountId = originAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
}
