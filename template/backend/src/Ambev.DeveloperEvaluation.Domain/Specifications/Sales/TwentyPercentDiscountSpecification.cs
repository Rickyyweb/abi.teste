using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales
{
    /// <summary>
    /// Regra de negócio: aplica desconto de 20% se uma venda tiver pelo menos 20 itens totais.
    /// </summary>
    public class TwentyPercentDiscountSpecification : ISpecification<Sale>
    {
        public bool IsSatisfiedBy(Sale sale)
        {
            if (sale == null)
                return false;

            // Soma a quantidade de todos os itens na venda
            var totalQuantidade = sale.Items.Sum(i => i.Quantity);

            return totalQuantidade >= 20;
        }
    }
}
