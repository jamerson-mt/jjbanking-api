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

    // TESTE DE REGISTRO DE CONTA NO BANCO (sucesso e cpf nao cadastrado)
    [Fact]
    public async Task AuthRegister_WhenDataIsValid_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            email = "teste123@gmail.com",
            password = "Teste@1234",
            fullName = "Teste de Registro",
            cpf = "09478206478", // O CPF DEVE SER UNICO PARA CADA TESTE, POIS A API NAO PERMITE DUPLICADOS
        };

        // Act
        // Envia a requisição POST para o endpoint de registro com os dados do usuário
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        // ADICIONE ESTE BLOCO PARA DEBUGAR:
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var contents = await response.Content.ReadAsStringAsync();
            throw new Exception($"A API REJEITOU O CADASTRO: {contents}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);

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

    // TESTE DE REGISTRO COM SENHA FRACA
    [Fact]
    public async Task Register_DeveRetornarBadRequest_QuandoSenhaForFraca()
    {
        // Arrange: Dados válidos, exceto a senha
        var request = new AccountRegister
        {
            Email = "mersonmt@gmail.com",
            Password = "1", // Senha impossível de passar
            FullName = "Teste Erro",
            Cpf = "09478206478", // CPF impossível de passar
        };

        // Act
        // Envia a requisição POST para o endpoint de registro com os dados do usuário
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        // Verifica se a API retorna BadRequest para uma senha fraca
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Lê o conteúdo da resposta para garantir que a API está explicando o motivo do erro
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        // Garante que a API explica o erro para o front-end
    }

    //  TESTE DE REGISTRO COM CPF INVÁLIDO
    [Fact]
    public async Task Register_DeveRetornarBadRequest_QuandoCpfForInvalido()
    {
        // Arrange: Dados válidos, exceto o CPF
        var request = new AccountRegister
        {
            Email = "mersonmt@gmail.com",
            Password = "Teste@1234", // Senha impossível de passar
            FullName = "Teste Erro",
            Cpf = "09478206477", // CPF impossível de passar
        };

        // Act
        // Envia a requisição POST para o endpoint de registro com os dados do usuário
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        // Verifica se a API retorna BadRequest para um CPF inválido
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //espera que o valor seja cpf invalido
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("CPF inválido. Informe um CPF realmente válido.");
    }

    //  TESTE DE REGISTRO COM CPF existente
    [Fact]
    public async Task Register_DeveRetornarBadRequest_QuandoCpfExistir()
    {
        // Arrange: Dados válidos, exceto o CPF
        var request = new AccountRegister
        {
            Email = "mersonmt@gmail.com",
            Password = "Teste@1234", // Senha impossível de passar
            FullName = "Teste Erro",
            Cpf = "09478206478", // CPF impossível de passar
        };

        // Act
        // Envia a requisição POST para o endpoint de registro com os dados do usuário
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        // Verifica se a API retorna BadRequest para um CPF inválido
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //espera que o valor seja cpf invalido
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("CPF já cadastrado. Informe um CPF diferente.");
    }
}
