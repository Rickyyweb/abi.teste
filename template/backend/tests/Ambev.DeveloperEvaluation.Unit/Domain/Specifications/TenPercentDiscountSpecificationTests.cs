using FluentAssertions;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications
{
    public class TenPercentDiscountSpecificationTests
    {
        [Fact(DisplayName = "TenPercentDiscountSpecification: Deve aplicar quando >= 10 itens")]
        public void IsSatisfiedBy_AtLeast10Items_ShouldBeTrue()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();
            sale.AddItem(pid, 10, 10m);

            // Act
            var spec = new TenPercentDiscountSpecification();
            var result = spec.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "TenPercentDiscountSpecification: Deve não aplicar quando < 10 itens")]
        public void IsSatisfiedBy_FewerThan10Items_ShouldBeFalse()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();
            sale.AddItem(pid, 5, 10m);

            // Act
            var spec = new TenPercentDiscountSpecification();
            var result = spec.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse();
        }
    }
}
