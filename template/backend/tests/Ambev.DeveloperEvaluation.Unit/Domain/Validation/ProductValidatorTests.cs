using FluentAssertions;
using Xunit;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.Unit.Validation
{
    public class ProductValidatorTests
    {
        [Fact(DisplayName = "ProductValidator: Deve falhar quando Name vazio")]
        public async Task Validator_NameEmpty_ShouldFail()
        {
            // Arrange
            var request = new CreateProductRequest { Name = string.Empty, Price = 10m };
            var validator = new CreateProductRequestValidator();

            // Act
            ValidationResult result = await validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateProductRequest.Name));
        }

        [Fact(DisplayName = "ProductValidator: Deve falhar quando Price <= 0")]
        public async Task Validator_PriceNonPositive_ShouldFail()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "Teste", Price = -1m };
            var validator = new CreateProductRequestValidator();

            // Act
            ValidationResult result = await validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should()
            .Contain(e => e.PropertyName == nameof(CreateProductRequest.Price))
            .Which.ErrorMessage.Should().Be("O campo 'Price' não pode ser negativo.");
        }
    }
}
