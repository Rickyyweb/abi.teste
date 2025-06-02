using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Configurations;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales
{
    public class SalesIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlTestcontainer _postgresContainer;

        static SalesIntegrationTests()
        {
            // Desabilita o Ryuk via variável de ambiente, antes de qualquer Testcontainers ser instanciado
            Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");
        }

        public SalesIntegrationTests()
        {
            _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithImage("postgres:13")
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "integration_test_db",
                    Username = "developer",
                    Password = "ev@luAt10n"
                })
                // Aguarda apenas até a porta 5432 estar disponível (Postgres pronto)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                // Sem tentar anexar logs, sem usar Ryuk, sem chamar WithCleanUp
                .Build();
        }

        public async Task InitializeAsync()
        {
            // StartAsync agora apenas aguarda a porta do Postgres ficar aberta,
            // sem tentar “hijack” de streams de logs
            await _postgresContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgresContainer.StopAsync();
        }

        private DefaultContext CreateContext()
        {
            var connStr = _postgresContainer.ConnectionString;
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseNpgsql(connStr, b =>
                    b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"))
                .Options;

            var ctx = new DefaultContext(options);
            // Aplica migrações no banco real do container
            ctx.Database.Migrate();
            return ctx;
        }

        [Fact(DisplayName = "CreateSale e GetSale via EFCore/Postgres real")]
        public async Task CreateAndGetSale_IntegrationTest()
        {
            // Arrange: cria contexto e aplica migrações no banco do container
            using var context = CreateContext();

            // Seed: adiciona dois produtos que serão usados no comando
            var prodA = new Product { Id = Guid.NewGuid(), Name = "Prod A", Price = 10m };
            var prodB = new Product { Id = Guid.NewGuid(), Name = "Prod B", Price = 20m };
            context.Products.AddRange(prodA, prodB);
            await context.SaveChangesAsync();

            // Configura DI: DefaultContext apontando para o container, MediatR e AutoMapper
            var services = new ServiceCollection();
            services.AddDbContext<DefaultContext>(o =>
                o.UseNpgsql(_postgresContainer.ConnectionString,
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

            // Cria o comando de criação de venda (uso de construtor posicional no DTO)
            var createCmd = new CreateSaleCommand(
                IdempotencyKey: "abc123",
                CustomerId: Guid.NewGuid(),
                Items: new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto(prodA.Id, 2),
                    new CreateSaleItemDto(prodB.Id, 1)
                }
            );

            // Act: envia o comando para criar a venda
            var created = await mediator.Send(createCmd);

            // Assert: verifica que o resultado contém exatamente 2 itens
            created.Should().NotBeNull();
            created.Items.Should().HaveCount(2);

            // Busca a venda criada via query
            var getQuery = new GetSaleQuery(created.Id);
            var fetched = await mediator.Send(getQuery);

            // Verifica que a venda buscada existe e também tem 2 itens
            fetched.Should().NotBeNull();
            fetched.Id.Should().Be(created.Id);
            fetched.Items.Should().HaveCount(2);
        }
    }
}
