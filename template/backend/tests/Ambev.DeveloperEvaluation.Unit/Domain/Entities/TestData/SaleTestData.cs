using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData
{
    internal static class SaleTestData
    {
        /// <summary>
        /// Retorna uma Sale “ativa” contendo 2 itens diferentes.
        /// </summary>
        public static Sale ValidSaleWithItems(Guid productA, Guid productB)
        {
            var sale = new Sale(customerId: Guid.NewGuid());

            // Supondo que cada AddItem lance exceção se quantity <= 0 ou > 20
            sale.AddItem(productA, quantity: 2, unitPrice: 10m);
            sale.AddItem(productB, quantity: 1, unitPrice: 20m);

            return sale;
        }

        /// <summary>
        /// Retorna uma Sale “vazia” (sem items).
        /// </summary>
        public static Sale EmptySale()
        {
            return new Sale(customerId: Guid.NewGuid());
        }

        /// <summary>
        /// Retorna uma Sale inválida com mais de 20 unidades do mesmo produto.
        /// </summary>
        public static Sale TooManySameItemsSale(Guid productId)
        {
            var sale = new Sale(customerId: Guid.NewGuid());
            // adiciona 21 itens de productId → deve exceder o limite de 20
            for (int i = 0; i < 21; i++)
            {
                sale.AddItem(productId, quantity: 1, unitPrice: 5m);
            }

            return sale;
        }
    }
}
