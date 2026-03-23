using System.Linq;
using JJBanking.Infra.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace JJBanking.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // builder.ConfigureServices(services =>
        // {
        //     // 1. Remova qualquer registro prévio do DbContext para evitar conflitos
        //     var descriptor = services.SingleOrDefault(d =>
        //         d.ServiceType == typeof(DbContextOptions<BankDbContext>)
        //     );
        //     if (descriptor != null)
        //         services.Remove(descriptor);

        //     // 2. Adicione o banco em memória com um nome ÚNICO por execução
        //     // Isso garante que cada teste comece com o banco zerado
        //     services.AddDbContext<BankDbContext>(options =>
        //     {
        //         options.UseInMemoryDatabase("JJBankingTestDb_" + Guid.NewGuid().ToString());
        //         options.ConfigureWarnings(x =>
        //             x.Ignore(
        //                 Microsoft
        //                     .EntityFrameworkCore
        //                     .Diagnostics
        //                     .InMemoryEventId
        //                     .TransactionIgnoredWarning
        //             )
        //         );
        //     });

        //     // 3. Inicialização do Banco de Dados
        //     var sp = services.BuildServiceProvider();
        //     using var scope = sp.CreateScope();
        //     var scopedServices = scope.ServiceProvider;
        //     var db = scopedServices.GetRequiredService<BankDbContext>();

        //     db.Database.EnsureDeleted(); // Limpa resquícios
        //     db.Database.EnsureCreated(); // Cria a estrutura
        // });
    }
}
