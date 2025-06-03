using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Helpers;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales
{
    [Collection(DockerIntegrationCollection.Name)]
    public class SalesIntegrationTestsDocker : IAsyncLifetime
    {
        private const string ContainerName = "postgres-integration-test";
        private const string ConnectionString = "Host=localhost;Port=5433;Database=integration_test_db;Username=developer;Password=ev@luAt10n";
        private Process? _dockerProcess;

        public async Task InitializeAsync()
        {
            // Para e remove container se existir
            await StopAndRemoveContainer();

            // Inicia novo container PostgreSQL
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"run -d --name {ContainerName} -p 5433:5432 " +
                           "-e POSTGRES_DB=integration_test_db " +
                           "-e POSTGRES_USER=developer " +
                           "-e POSTGRES_PASSWORD=ev@luAt10n " +
                           "postgres:13-alpine",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            _dockerProcess = Process.Start(startInfo);
            await _dockerProcess!.WaitForExitAsync();

            if (_dockerProcess.ExitCode != 0)
            {
                var error = await _dockerProcess.StandardError.ReadToEndAsync();
                throw new InvalidOperationException($"Falha ao iniciar container Docker: {error}");
            }

            // Aguarda o PostgreSQL ficar pronto
            await WaitForPostgres();
        }

        public async Task DisposeAsync()
        {
            await StopAndRemoveContainer();
        }

        private async Task StopAndRemoveContainer()
        {
            try
            {
                // Para o container
                var stopProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"stop {ContainerName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                });
                await stopProcess!.WaitForExitAsync();

                // Remove o container
                var removeProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"rm {ContainerName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                });
                await removeProcess!.WaitForExitAsync();
            }
            catch
            {
                // Ignora erros de cleanup
            }
        }

        private async Task WaitForPostgres()
        {
            const int maxAttempts = 30;
            const int delayMs = 1000;

            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    using var context = CreateContext();
                    await context.Database.CanConnectAsync();
                    return; // Conexão bem-sucedida
                }
                catch
                {
                    if (i == maxAttempts - 1)
                        throw new TimeoutException("PostgreSQL não ficou pronto em tempo hábil");

                    await Task.Delay(delayMs);
                }
            }
        }

        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseNpgsql(ConnectionString, b =>
                    b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"))
                .Options;

            var ctx = new DefaultContext(options);
            ctx.Database.Migrate();
            return ctx;
        }

        [Fact(DisplayName = "CreateSale e GetSale via Docker PostgreSQL")]
        public async Task CreateAndGetSale_IntegrationTest()
        {
            // Arrange
            using var context = CreateContext();

            var prodA = new Product { Id = Guid.NewGuid(), Name = "Prod A", Price = 10m };
            var prodB = new Product { Id = Guid.NewGuid(), Name = "Prod B", Price = 20m };
            context.Products.AddRange(prodA, prodB);
            await context.SaveChangesAsync();

            // Configura DI
            var services = new ServiceCollection();
            services.AddDbContext<DefaultContext>(o =>
                o.UseNpgsql(ConnectionString,
                            b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(CreateSaleCommand).Assembly,
                    typeof(GetSaleQuery).Assembly
                );
            });

            services.AddLogging();
            services.AddAutoMapper(
                typeof(CreateSaleProfile),
                typeof(GetSaleProfile)
            );

            var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();

            // Act
            var createCmd = new CreateSaleCommand(
                IdempotencyKey: Guid.NewGuid().ToString(),
                CustomerId: Guid.NewGuid(),
                Items: new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto(prodA.Id, 2),
                    new CreateSaleItemDto(prodB.Id, 1)
                }
            );

            var created = await mediator.Send(createCmd);

            // Assert
            created.Should().NotBeNull();
            created.Items.Should().HaveCount(2);

            var getQuery = new GetSaleQuery(created.Id);
            var fetched = await mediator.Send(getQuery);

            fetched.Should().NotBeNull();
            fetched.Id.Should().Be(created.Id);
            fetched.Items.Should().HaveCount(2);

        }
    }
}