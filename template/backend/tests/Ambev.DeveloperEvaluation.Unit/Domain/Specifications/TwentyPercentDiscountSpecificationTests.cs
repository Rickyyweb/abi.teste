using FluentAssertions;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications
{
    public class TwentyPercentDiscountSpecificationTests
    {
        [Fact(DisplayName = "TwentyPercentDiscountSpecification: Deve aplicar quando >= 20 itens")]
        public void IsSatisfiedBy_AtLeast20Items_ShouldBeTrue()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();
            sale.AddItem(pid, 20, 10m);

            // Act
            var spec = new TwentyPercentDiscountSpecification();
            var result = spec.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "TwentyPercentDiscountSpecification: Deve não aplicar quando < 20 itens")]
        public void IsSatisfiedBy_LessThan20Items_ShouldBeFalse()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();
            sale.AddItem(pid, 15, 10m);

            // Act
            var spec = new TwentyPercentDiscountSpecification();
            var result = spec.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse();
        }
    }
}
