using FluentAssertions;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleTests
    {
        [Fact(DisplayName = "Sale: Limite de 20 itens iguais deve lançar erro")]
        public void AddItem_Exceed20SameProduct_ShouldThrow()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();

            // Adiciona 20 itens válidos
            for (int i = 0; i < 20; i++)
            {
                sale.AddItem(pid, 1, 10m);
            }

            // Act
            Action act = () => sale.AddItem(pid, 1, 10m);

            // Assert
            act.Should()
               .Throw<DomainException>()
               .WithMessage("*20 unidades do mesmo item*");
        }

        [Fact(DisplayName = "Sale: Calcular Subtotal, Desconto e Total corretamente")]
        public void Sale_CalculateTotals_ShouldBeCorrect()
        {
            // Arrange
            var sale = new Sale(Guid.NewGuid());
            var pid = Guid.NewGuid();

            sale.AddItem(pid, 10, 10m); // subtotal = 100, desconto 10% se regra = 10 itens
            // A lógica interna deve definir desconto = 10, total = 90

            // Act
            var subtotal = sale.Subtotal;
            var discount = sale.Discount;
            var total = sale.TotalAmount;

            // Assert
            subtotal.Should().Be(100m);
            discount.Should().Be(10m);
            total.Should().Be(90m);
        }
    }
}
