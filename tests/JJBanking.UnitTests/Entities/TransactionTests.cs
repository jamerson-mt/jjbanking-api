using FluentAssertions;
using JJBanking.Domain.Entities;
using JJBanking.Domain.Enums;
using Xunit;

namespace JJBanking.UnitTests.Entities;

public class TransactionTests
{
    // TESTE PARA VER SE O CONSTRUTOR DA TRANSAÇÃO FUNCIONA CORRETAMENTE
    [Fact]
    public void Constructor_WhenValidData_ShouldCreateTransaction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 150.00m;
        var type = TransactionType.Credit;
        var description = "Depósito inicial";

        // Act
        var transaction = new Transaction(accountId, amount, type, description);

        // Assert
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(type);
        transaction.Description.Should().Be(description);
        transaction.Id.Should().NotBeEmpty(); // Garante que o GUID foi gerado
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1)); // Verifica se a data de criação é recente
    }

    // TESTE PARA VER SE O CONSTRUTOR DA TRANSAÇÃO LIDA COM VALORES NEGATIVOS
    [Theory]
    [InlineData(-50.00, TransactionType.Credit)] // Testa um valor negativo para depósito
    [InlineData(-100.00, TransactionType.Debit)] // Testa um valor negativo para saque
    public void Constructor_WhenAmountIsNegative_ShouldThrowException(
        decimal amount,
        TransactionType type
    )
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var description = "Teste de valor negativo";

        // Act
        Action act = () => new Transaction(accountId, amount, type, description);

        // Assert
        act.Should()
            .Throw<ArgumentException>() // Espera que uma ArgumentException seja lançada
            .WithMessage("O valor da transação não pode ser negativo. (Parameter 'amount')");
    }

    // TESTE PARA VER SE O CONSTRUTOR DA TRANSAÇÃO LIDA COM DESCRIÇÕES MUITO LONGAS
    [Fact]
    public void Constructor_WhenDescriptionIsTooLong_ShouldThrowException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 100.00m;
        var type = TransactionType.Credit;
        var description = new string('A', 256); // Cria uma descrição com 256 caracteres

        // Act
        Action act = () => new Transaction(accountId, amount, type, description);

        // Assert
        act.Should()
            .Throw<ArgumentException>() // Espera que uma ArgumentException seja lançada
            .WithMessage("A descrição é obrigatória e deve ter no máximo 250 caracteres.");
    }

    // valor nao pode ser 0
    [Fact]
    public void Constructor_WhenAmountIsZero_ShouldThrowException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 0.00m; // Valor zero
        var type = TransactionType.Credit;
        var description = "Teste de valor zero";

        // Act
        Action act = () => new Transaction(accountId, amount, type, description);

        // Assert
        act.Should()
            .Throw<ArgumentException>() // Espera que uma ArgumentException seja lançada
            .WithMessage("O valor da transação não pode ser negativo. (Parameter 'amount')");
    }
    // tem que ter um recebbedor    
    [Fact]
    public void Constructor_WhenAccountIdIsEmpty_ShouldThrowException()
    {
        // Arrange
        var accountId = Guid.Empty; // ID de conta vazio
        var amount = 100.00m;
        var type = TransactionType.Credit;
        var description = "Teste de conta vazia";

        // Act
        Action act = () => new Transaction(accountId, amount, type, description);

        // Assert
        act.Should()
            .Throw<ArgumentException>() // Espera que uma ArgumentException seja lançada
            .WithMessage("O Destinatário da transação é obrigatório. (Parameter 'accountId')");
    }
}
