using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JJBanking.IntegrationTests.Controllers;

// Program é a classe principal da sua API
public class AccountsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountsControllerTests(WebApplicationFactory<Program> factory)
    {
        // Cria um "cliente" que sabe conversar com a sua API em memória
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAccount_WhenDataIsValid_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            Owner = "Jamerson Teste",
            Cpf = "99988877763",
            InitialDeposit = 100.00m,
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Lendo o JSON usando o molde AccountResponse
        var content = await response.Content.ReadFromJsonAsync<AccountResponse>();

        content.Should().NotBeNull();
        content!.Id.Should().NotBeEmpty();
        content.Owner.Should().Be(request.Owner);
    }
}

// Coloque isso no final do arquivo, fora da classe AccountTests
public record AccountResponse(Guid Id, string Owner, string Cpf, decimal Balance); // 
