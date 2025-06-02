using Ambev.DeveloperEvaluation.Domain.Validation.Product;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class ProductTests
    {
        [Fact(DisplayName = "Product: Nome obrigatório")]
        public void Product_NameVazio_DeveFalharValidator()
        {
            // Arrange
            var p = ProductTestData.ValidProduct();
            p.Name = string.Empty;
            var validator = new ProductValidator();

            // Act
            var result = validator.Validate(p);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors
                  .Should()
                  .Contain(e => e.PropertyName == nameof(p.Name))
                  .Which.ErrorMessage
                  .Should()
                  .Contain("Name");
        }
    }
}
