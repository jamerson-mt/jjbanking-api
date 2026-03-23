using System.Reflection; // Adicione este using no topo
using JJBanking.Domain.DTOs;
using JJBanking.Domain.Entities;
using JJBanking.Domain.Interfaces;
using JJBanking.Infra.Context;
using JJBanking.Infra.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- BANCO DE DADOS (Configuração Condicional) ---
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<BankDbContext>(options =>
    {
        options.UseInMemoryDatabase("TestDb");
        options.ConfigureWarnings(x =>
            x.Ignore(
                Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning
            )
        );
    });
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<BankDbContext>(options => options.UseNpgsql(connectionString));
}

// --- IDENTITY (Configuração Essencial) ---
builder
    .Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<BankDbContext>()
    .AddDefaultTokenProviders();

// --- CONFIGURAÇÃO DE CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowLocalhost",
        policy =>
        {
            policy
                .AllowAnyOrigin() // Em localhost, permitir qualquer origem facilita o dev
                .AllowAnyHeader() // Permite qualquer header, útil para tokens e custom headers do React Native
                .AllowAnyMethod(); // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
        }
    );
});

// --- SERVIÇOS ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "JJ Banking API",
            Version = "v1",
            Description = "Documentação da API para integração com React Native.",
        }
    );

    // 1. Pega o nome do arquivo XML gerado pelo compilador
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    // 2. Localiza o caminho físico dele
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // 3. Verifica se o arquivo existe antes de tentar ler (evita erros no deploy)
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// --- PIPELINE ---
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
