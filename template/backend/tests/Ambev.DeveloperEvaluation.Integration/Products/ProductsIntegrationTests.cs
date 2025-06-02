using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Products
{
    public class ProductsIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlTestcontainer _postgresContainer;

        public ProductsIntegrationTests()
        {
            // Construímos um container especificamente do tipo PostgreSqlTestcontainer
            _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "integration_test_db",
                    Username = "developer",
                    Password = "ev@luAt10n"
                })
                .WithImage("postgres:13")
                .Build();
        }

        public async Task InitializeAsync() => await _postgresContainer.StartAsync();
        public async Task DisposeAsync() => await _postgresContainer.StopAsync();

        private DefaultContext CreateContext()
        {
            var connStr = _postgresContainer.ConnectionString;
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseNpgsql(connStr, b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"))
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

            // — 1) Monta ServiceCollection para MediatR + AutoMapper + DbContext
            var services = new ServiceCollection();

            services.AddDbContext<DefaultContext>(opts =>
                opts.UseNpgsql(_postgresContainer.ConnectionString,
                               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"))
            );

            // — 2) Registra MediatR (v12.x, sem o Extensions incompatível)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(CreateProductCommand).Assembly,
                    typeof(GetProductQuery).Assembly
                );
            });

            // — 3) Registra AutoMapper profiles
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new CreateProductProfile());
                cfg.AddProfile(new GetProductProfile());
            });

            // — 4) Registra logging genérico (caso seus Handlers usem ILogger)
            services.AddLogging();

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
    }
}
