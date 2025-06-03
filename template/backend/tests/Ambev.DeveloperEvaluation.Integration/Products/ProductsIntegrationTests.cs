using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Integration.Helpers;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Products
{
    [Collection(DockerIntegrationCollection.Name)]
    public class ProductsIntegrationTests : IAsyncLifetime
    {
        private const string ContainerName = "postgres-integration-test-products";
        private const string ConnectionString = "Host=localhost;Port=5434;Database=integration_test_db-products;Username=developer;Password=ev@luAt10n";
        private Process? _dockerProcess;

        static ProductsIntegrationTests()
        {
            // Desabilita o Ryuk via variável de ambiente, antes de qualquer Testcontainers ser instanciado
            Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");
        }
        public async Task InitializeAsync()
        {
            // Para e remove container se existir
            await StopAndRemoveContainer();

            // Inicia novo container PostgreSQL
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"run -d --name {ContainerName} -p 5434:5432 " +
                           "-e POSTGRES_DB=integration_test_db-products " +
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

        [Fact(DisplayName = "CreateProduct e GetProduct via Postgres real")]
        public async Task CreateAndGetProduct_IntegrationTest()
        {
            // — Arrange: contexto real com o Postgres em container
            using var context = CreateContext();

            var services = new ServiceCollection();
            services.AddDbContext<DefaultContext>(o =>
                o.UseNpgsql(ConnectionString,
                            b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")));

            // — 2) Registra MediatR (v12.x, sem o Extensions incompatível)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(CreateProductCommand).Assembly,
                    typeof(GetProductQuery).Assembly
                );
            });

            //services.AddTransient<Application.Interfaces.IEmailSender, SmtpEmailSender>();
            services.AddTransient<Application.Interfaces.IEmailSender, FakeEmailSender>();

            services.AddLogging();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new CreateProductProfile());
                cfg.AddProfile(new GetProductProfile());
            });

            var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();

            // — Act: cria um produto através do MediatR (CreateProductHandler)
            var cmd = new CreateProductCommand(Name: "Produto X", Price: 99.99m);
            var created = await mediator.Send(cmd);

            created.Should().NotBeNull();
            created.Name.Should().Be(cmd.Name);
            created.Price.Should().Be(cmd.Price);

            // — Act: busca o produto recém-criado (GetProductHandler)
            var query = new GetProductQuery(created.Id);
            var fetched = await mediator.Send(query);

            // — Assert: verifica se os dados batem
            fetched.Should().NotBeNull();
            fetched.Id.Should().Be(created.Id);
            fetched.Name.Should().Be(created.Name);
            fetched.Price.Should().Be(created.Price);

        }

        public class FakeEmailSender : Application.Interfaces.IEmailSender
        {
            private readonly ILogger<FakeEmailSender>? _logger;
            public List<EmailSent> EmailsSent { get; } = new();

            public FakeEmailSender(ILogger<FakeEmailSender>? logger = null)
            {
                _logger = logger;
            }

            public Task SendEmailAsync(string to, string subject, string body)
            {
                var email = new EmailSent
                {
                    To = to,
                    Subject = subject,
                    Body = body,
                    SentAt = DateTime.UtcNow
                };

                EmailsSent.Add(email);

                _logger?.LogInformation("Email FAKE enviado para {To}: {Subject}", to, subject);

                return Task.CompletedTask;
            }

            public class EmailSent
            {
                public string To { get; set; } = string.Empty;
                public string Subject { get; set; } = string.Empty;
                public string Body { get; set; } = string.Empty;
                public DateTime SentAt { get; set; }
            }
        }

    }
}
