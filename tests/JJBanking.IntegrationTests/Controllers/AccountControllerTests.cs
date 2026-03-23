using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JJBanking.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JJBanking.IntegrationTests.Controllers;

// Program é a classe principal da sua API
public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private string GenerateRandomCpf() =>
        Random.Shared.Next(100000000, 999999999).ToString() + "00";

    private readonly HttpClient _client;

    public AccountControllerTests(WebApplicationFactory<Program> factory)
    {
        // Cria um "cliente" que sabe conversar com a sua API em memória
        _client = factory.CreateClient();
    }
}
