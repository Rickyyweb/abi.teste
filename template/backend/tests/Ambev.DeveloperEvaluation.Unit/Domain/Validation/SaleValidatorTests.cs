using FluentAssertions;
using FluentValidation.Results;
using Xunit;

// IMPORTANTE: é aqui que dizemos ao compilador onde estão CreateSaleRequest e CreateSaleItemDto
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.Unit.Validation
{
    public class SaleValidatorTests
    {
        [Fact(DisplayName = "SaleValidator: Deve falhar quando CustomerId vazio")]
        public async Task Validator_CustomerIdEmpty_ShouldFail()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CustomerId = Guid.Empty,
                Items = new List<CreateSaleItemDto>
                {
                    // Agora criamos CreateSaleItemDto diretamente, não nested
                    new CreateSaleItemDto
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 1
                    }
                }
            };

            var validator = new CreateSaleRequestValidator();

            // Act
            ValidationResult result = await validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateSaleRequest.CustomerId));
        }

        [Fact(DisplayName = "SaleValidator: Deve falhar quando nenhum item fornecido")]
        public async Task Validator_NoItems_ShouldFail()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateSaleItemDto>() // lista vazia
            };

            var validator = new CreateSaleRequestValidator();

            // Act
            ValidationResult result = await validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateSaleRequest.Items));
        }

        [Fact(DisplayName = "SaleValidator: Deve falhar quando item com quantity <= 0")]
        public async Task Validator_ItemQuantityNonPositive_ShouldFail()
        {
            // Arrange
            var request = new CreateSaleRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 0
                    }
                }
            };

            var validator = new CreateSaleRequestValidator();

            // Act
            ValidationResult result = await validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            // O PropertyName para quantidade virá como “Items[0].Quantity”
            result.Errors.Should().Contain(e => e.PropertyName.EndsWith("Items[0].Quantity"));
        }
    }
}
