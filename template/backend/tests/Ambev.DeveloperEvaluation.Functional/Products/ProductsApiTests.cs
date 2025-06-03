using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Products
{
    //public class ProductsApiTests : IClassFixture<ProductsApiTests.CustomWebAppFactory>
    //{
    //    private readonly HttpClient _client;

    //    public ProductsApiTests(CustomWebAppFactory factory)
    //    {
    //        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    //        {
    //            BaseAddress = new Uri("http://localhost")
    //        });
    //    }

        //[Fact(DisplayName = "POST /api/Products cria e GET /api/Products/{id} retorna o mesmo")]
        //public async Task PostProduct_ThenGetProduct_ReturnsSameData()
        //{
        //    // Arrange: montar o request de criação
        //    var createRequest = new CreateProductRequest
        //    {
        //        Name = "Produto Func",
        //        Price = 15.5m
        //    };

        //    // Act: POST /api/Products
        //    var postResponse = await _client.PostAsJsonAsync("/api/Products", createRequest);
        //    postResponse.EnsureSuccessStatusCode();

        //    var created = await postResponse
        //        .Content
        //        .ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();

        //    created.Should().NotBeNull();
        //    created!.Data.Id.Should().NotBeEmpty();
        //    created.Data.Name.Should().Be(createRequest.Name);
        //    created.Data.Price.Should().Be(createRequest.Price);

        //    // Act: GET /api/Products/{id}
        //    var productId = created.Data.Id;
        //    var getResponse = await _client.GetAsync($"/api/Products/{productId}");
        //    getResponse.EnsureSuccessStatusCode();

        //    var fetched = await getResponse
        //        .Content
        //        .ReadFromJsonAsync<ApiResponseWithData<GetProductResponse>>();

        //    fetched.Should().NotBeNull();
        //    fetched!.Data.Id.Should().Be(productId);
        //    fetched.Data.Name.Should().Be(createRequest.Name);
        //    fetched.Data.Price.Should().Be(createRequest.Price);
        //}

        /// <summary>
        /// Fábrica customizada que substitui DefaultContext por InMemory, 
        /// de modo a não usar o PostgreSQL real ao subir a aplicação em memória.
        /// </summary>
        //public class CustomWebAppFactory : WebApplicationFactory<Program>
        //{
        //    protected override IHost CreateHost(IHostBuilder builder)
        //    {
        //        // 1) Antes de construir, remova o registro de DefaultContext e adicione InMemory
        //        builder.ConfigureServices(services =>
        //        {
        //            // 1.1) Remove qualquer DbContextOptions<DefaultContext> já registrado
        //            var descriptor = services.SingleOrDefault(d =>
        //                d.ServiceType == typeof(DbContextOptions<DefaultContext>));
        //            if (descriptor != null)
        //            {
        //                services.Remove(descriptor);
        //            }

        //            // 1.2) Remove também o DefaultContext em si (caso tenha sido registrado diretamente)
        //            var contextDescriptor = services.SingleOrDefault(d =>
        //                d.ServiceType == typeof(DefaultContext));
        //            if (contextDescriptor != null)
        //            {
        //                services.Remove(contextDescriptor);
        //            }

        //            // 1.3) Registra DefaultContext usando InMemory
        //            services.AddDbContext<DefaultContext>(options =>
        //            {
        //                options.UseInMemoryDatabase("InMemoryTestDb");
        //            });

        //            // 1.4) Reconstrói o provedor para garantir que o banco InMemory seja criado
        //            var sp = services.BuildServiceProvider();
        //            using (var scope = sp.CreateScope())
        //            {
        //                var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        //                db.Database.EnsureDeleted();
        //                db.Database.EnsureCreated();
        //            }
        //        });

        //        return base.CreateHost(builder);
        //    }
        //}
   // }
}
