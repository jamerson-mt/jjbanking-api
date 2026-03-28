using System.Reflection;
using JJBanking.Domain.DTOs;
using JJBanking.Domain.Entities;
using JJBanking.Domain.Interfaces;
using JJBanking.Infra.Context;
using JJBanking.Infra.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- BANCO DE DADOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BankDbContext>(options => options.UseSqlite(connectionString));

// --- IDENTITY ---
builder
    .Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<BankDbContext>()
    .AddDefaultTokenProviders();

// --- CONFIGURAÇÃO DE CORS (AJUSTADA) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "ProductionPolicy",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

// --- SERVIÇOS ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// --- MIGRATIONS E CRIAÇÃO DE DIRETÓRIO ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var connString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connString) && connString.Contains("Data Source="))
        {
            var dbPath = connString.Replace("Data Source=", "");
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        var context = services.GetRequiredService<BankDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro nas migrations.");
    }
}

// --- PIPELINE DE MIDDLEWARE (ORDEM IMPORTANTE) ---

app.UseSwagger();
app.UseSwaggerUI();

// 1. Roteamento deve vir primeiro
app.UseRouting();

// 2. CORS deve vir após o Routing e ANTES da Autenticação
app.UseCors("ProductionPolicy");

// 3. Segurança
app.UseAuthentication();
app.UseAuthorization();

// 4. Endpoints
app.MapControllers();

app.Run();
