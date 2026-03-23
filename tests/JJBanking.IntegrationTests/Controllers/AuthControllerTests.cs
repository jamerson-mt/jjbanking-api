using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JJBanking.Domain.DTOs;
using JJBanking.Domain.Entities;

namespace JJBanking.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(); //
    }

    // Gerador de CPF
    private string GenerateRandomCpf() =>
        Random.Shared.Next(100000000, 999999999).ToString() + "00";

    // TESTE DE REGISTRO DE CONTA NO BANCO
    [Fact]
    public async Task AuthRegister_WhenDataIsValid_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            email = "teste123@gmail.com",
            password = "Teste@1234",
            fullName = "Teste de Registro",
            cpf = "094782999999", // O CPF DEVE SER UNICO PARA CADA TESTE, POIS A API NAO PERMITE DUPLICADOS
        };

        // Act
        // Envia a requisição POST para o endpoint de registro com os dados do usuário
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        // ADICIONE ESTE BLOCO PARA DEBUGAR:
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var contents = await response.Content.ReadAsStringAsync();
            throw new Exception($"A API REJEITOU O CADASTRO: {contents}");
        }

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Lendo o JSON usando o molde AuthResponse (que é o que a API retorna)
        // verifica se o token, número da conta e nome do usuário estão presentes e corretos
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrEmpty();
        content.AccountNumber.Should().NotBeNullOrEmpty();
        content.UserName.Should().Be(request.fullName);
    }
}
