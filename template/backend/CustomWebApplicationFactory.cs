using Ambev.DeveloperEvaluation.ORM.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Functional
{
    /// <summary>
    /// Fabrica customizada para testes que substitui o DefaultContext
    /// real por um InMemoryDatabase, de modo a não tocar no PostgreSQL real.
    /// </summary>
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // 1) Antes de construir o host, remova e substitua o DbContext
            builder.ConfigureServices(services =>
            {
                // 1.1) Encontre a descrição do DbContext real (DefaultContext)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 1.2) Adicione o DbContext com InMemory
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // 1.3) Garante que o provedor de migrações seja removido (se houver).
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });

            return base.CreateHost(builder);
        }
    }
}
