using Ambev.DeveloperEvaluation.Functional.CustomWebApplicationFactory;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales
{
    // Note que agora usamos CustomWebApplicationFactory<Program>
    public class SalesApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SalesApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }


        [Fact(DisplayName = "Funcional: POST /api/Sales e GET /api/Sales/{id}")]
        public async Task PostSale_ThenGetSale_ReturnsSameData()
        {
            // 1) Criar um produto via API
            var createProdRequest = new CreateProductRequest
            {
                Name = "Produto Func A",
                Price = 10m
            };

            var prodResponse = await _client.PostAsJsonAsync("/api/Products", createProdRequest);
            prodResponse.EnsureSuccessStatusCode();

            var createdProd = await prodResponse
                .Content
                .ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
            var prodId = createdProd!.Data.Id;

            // 2) Criar a venda (use o prodId retornado acima)
            var createSaleRequest = new CreateSaleRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto
                    {
                        ProductId = prodId,
                        Quantity = 2,
                    }
                }
            };

            var saleResponse = await _client.PostAsJsonAsync("/api/Sales", createSaleRequest);
            saleResponse.EnsureSuccessStatusCode();

            var createdSale = await saleResponse
                .Content
                .ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
            var saleId = createdSale!.Data.Id;

            // 3) GET /api/Sales/{saleId}
            var getResponse = await _client.GetAsync($"/api/Sales/{saleId}");
            getResponse.EnsureSuccessStatusCode();

            var fetchedSale = await getResponse
                .Content
                .ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
            fetchedSale.Should().NotBeNull();
            fetchedSale!.Data.Id.Should().Be(saleId);
            fetchedSale.Data.Items.Should().HaveCount(1);
        }
    }
}
