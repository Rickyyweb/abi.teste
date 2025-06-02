using Ambev.DeveloperEvaluation.ORM.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Functional.CustomWebApplicationFactory
{
    ///// <summary>
    /// Fabrica customizada para testes que:
    /// 1) Sobe recursivamente até encontrar Ambev.DeveloperEvaluation.WebApi.csproj, usando UseContentRoot.
    /// 2) Usa Environment = "Testing" (para que as migrations sejam puladas).
    /// 3) Remove o DefaultContext real (PostgreSQL) e registra um DefaultContext InMemory.
    /// 4) Garante que o banco InMemory seja recriado limpo a cada inicialização.
    /// </summary>
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 1) Descobre onde está o .csproj do WebApi (sem números fixos de "..")
            var projectDir = FindWebApiProjectDir();
            if (projectDir == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível localizar 'Ambev.DeveloperEvaluation.WebApi.csproj' " +
                    "subindo a partir de " + AppContext.BaseDirectory);
            }

            // 2) Diz ao Host de teste que o ContentRoot é exatamente a pasta do WebApi
            builder.UseContentRoot(projectDir);

            // 3) Força o ambiente como “Testing”, para que o Program.Main pule migrações
            builder.UseEnvironment("Testing");

            // 4) Substitui o DefaultContext real (Postgres) por InMemory
            builder.ConfigureServices(services =>
            {
                // 4.1) Remove o registro original de DbContextOptions<DefaultContext>
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 4.2) Registra DefaultContext usando InMemoryDatabase
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // 4.3) Garante que o InMemory seja recriado limpo
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }

        /// <summary>
        /// Sobe na hierarquia de pastas até encontrar “src\Ambev.DeveloperEvaluation.WebApi\Ambev.DeveloperEvaluation.WebApi.csproj”.
        /// </summary>
        private string? FindWebApiProjectDir()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                // Ajuste aqui se o seu WebApi estiver em outro subcaminho
                var candidate = Path.Combine(
                    current.FullName,
                    "src",
                    "Ambev.DeveloperEvaluation.WebApi",
                    "Ambev.DeveloperEvaluation.WebApi.csproj"
                );
                if (File.Exists(candidate))
                {
                    return Path.GetDirectoryName(candidate)!;
                }
                current = current.Parent;
            }
            return null;
        }
    }
}
