using JJBanking.Infra.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações da JJ Banking API
// Adiciona o suporte a Controllers (essencial para API)
builder.Services.AddControllers();

// Configura o PostgreSQL usando a ConnectionString do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BankDbContext>(options => options.UseNpgsql(connectionString));

// Configura o Swagger/OpenAPI para documentação
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Pipeline de Execução
// Habilita o Swagger independente do ambiente para facilitar o seu teste e o de quem clonar
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JJ Banking API v1");
    c.RoutePrefix = "swagger"; // Acessível em http://localhost:5000/swagger
});

// 3. Auto-Migrations (Garante que o banco nasça no Docker)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BankDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrations no PostgreSQL.");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapeia os Controllers que vamos criar
app.MapControllers();

app.Run();
