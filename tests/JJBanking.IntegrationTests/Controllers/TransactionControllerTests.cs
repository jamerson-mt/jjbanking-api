// crie classe para TransactionControllerTests seguindo o modelo de AccountControllerTests, mas testando os endpoints de depósito e saque da TransactionController
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JJBanking.API.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JJBanking.IntegrationTests.Controllers;

public class TransactionControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TransactionControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // TESTE PARA VER SE O ENDPOINT DE DEPÓSITO FUNCIONA CORRETAMENTE
    [Fact]
    public async Task Deposit_WhenDataIsValid_ShouldReturnOk()
    {
        // Arrange
        var accountRequest = new
        {
            Owner = "Jamerson Teste",
            Cpf = GenerateRandomCpf(),
            InitialDeposit = 100.00m,
        };

        var httpResponse = await _client.PostAsJsonAsync("/api/accounts", accountRequest); // Cria uma conta para usar o ID no depósito
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created); // Verifica se a conta foi criada com sucesso

        var accountResponse = await httpResponse.Content.ReadFromJsonAsync<AccountResponse>(); // Lê a resposta para obter o ID da conta criada

        var depositRequest = new DepositRequest(
            accountResponse!.Id, // Usa o ID da conta criada para o depósito. o "!" é para informar que o accountResponse não será nulo, já que a conta foi criada com sucesso
            50.00m,
            "Depósito de teste"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/transaction/deposit", depositRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<TransactionResponse>(); // Lê a resposta do depósito para verificar se os dados estão corretos

        content.Should().NotBeNull(); // não deve ser nulo

        content!.Amount.Should().Be(depositRequest.Amount); // o valor do depósito deve ser igual ao que foi enviado na requisição
        content.Description.Should().Be(depositRequest.Description); // a descrição do depósito deve ser igual à que foi enviada na requisição
    }

    // TESTE DE SAQUE, SE O ENDPOINT DE SAQUE FUNCIONA CORRETAMENTE E SE O VALOR DO SAQUE É DEDUZIDO CORRETAMENTE DO SALDO DA CONTA
    [Fact]
    public async Task Withdraw_WhenDataIsValid_ShouldReturnOk()
    {
        // Arrange
        var accountRequest = new
        {
            Owner = "Jamerson Teste",
            Cpf = GenerateRandomCpf(),
            InitialDeposit = 100.00m,
        };

        var httpResponse = await _client.PostAsJsonAsync("/api/accounts", accountRequest); // Cria uma conta para usar o ID no saque
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created); // Verifica se a conta foi criada com sucesso

        var accountResponse = await httpResponse.Content.ReadFromJsonAsync<AccountResponse>(); // Lê a resposta para obter o ID da conta criada

        var withdrawRequest = new TransationWithdrawRequest(
            accountResponse!.Id, // Usa o ID da conta criada para o saque. o "!" é para informar que o accountResponse não será nulo, já que a conta foi criada com sucesso
            30.00m,
            "Saque de teste"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/transaction/withdraw", withdrawRequest); // Envia a requisição de saque para o endpoint de saque da TransactionController

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK); // Verifica se a resposta do saque foi OK (200)

        var content = await response.Content.ReadFromJsonAsync<TransactionResponse>(); // Lê a resposta do saque para verificar se os dados estão corretos

        content.Should().NotBeNull(); // não deve ser nulo

        content!.Amount.Should().Be(withdrawRequest.Amount); // o valor do saque deve ser igual ao que foi enviado na requisição
        content.Description.Should().Be(withdrawRequest.Description); // a descrição do saque deve ser igual à que foi enviada na requisição
    }

    // 🚀 MELHORIA: Testando múltiplos cenários inválidos (Zero e Negativo) com Theory
    [Theory]
    [InlineData(0)]
    [InlineData(-10.50)]
    public async Task Withdraw_WhenAmountIsInvalid_ShouldReturnBadRequest(decimal invalidAmount)
    {
        // Arrange: Criar a conta necessária para o teste
        var accountRequest = new CreatedAccountResponse(
            "Jamerson Teste",
            GenerateRandomCpf(),
            100.00m
        );

        var accountResponseMsg = await _client.PostAsJsonAsync("/api/accounts", accountRequest);
        accountResponseMsg.StatusCode.Should().Be(HttpStatusCode.Created);

        var accountData = await accountResponseMsg.Content.ReadFromJsonAsync<AccountResponse>();
        accountData.Should().NotBeNull(); // Segurança: Garante que temos os dados antes de prosseguir

        var withdrawRequest = new TransationWithdrawRequest(
            accountData!.Id,
            invalidAmount,
            "Tentativa de saque inválido"
        );

        // Act: Tenta realizar o saque
        var response = await _client.PostAsJsonAsync("/api/transaction/withdraw", withdrawRequest);

        // Assert: Valida se a API barrou a transação
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // OPCIONAL: Validar se a mensagem de erro retornada é a esperada
        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("positivo");
    }

    // TESTE DE SALDO INSUFICIENTE, SE O ENDPOINT DE SAQUE FUNCIONA CORRETAMENTE E SE O VALOR DO SAQUE É DEDUZIDO CORRETAMENTE DO SALDO DA CONTA
    [Fact]
    public async Task Withdraw_WhenBalanceIsInsufficient_ShouldReturnBadRequest()
    {
        // Arrange
        var accountRequest = new
        {
            Owner = "Jamerson Teste",
            Cpf = GenerateRandomCpf(),
            InitialDeposit = 50.00m, // Saldo inicial baixo para garantir insuficiência
        };

        var httpResponse = await _client.PostAsJsonAsync("/api/accounts", accountRequest);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var accountResponse = await httpResponse.Content.ReadFromJsonAsync<AccountResponse>();

        var withdrawRequest = new TransationWithdrawRequest(
            accountResponse!.Id,
            100.00m, // Valor do saque maior que o saldo disponível
            "Tentativa de saque com saldo insuficiente"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/transaction/withdraw", withdrawRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("Saldo insuficiente");
    }

    private string GenerateRandomCpf() =>
        Random.Shared.Next(100000000, 999999999).ToString() + "00";
}
