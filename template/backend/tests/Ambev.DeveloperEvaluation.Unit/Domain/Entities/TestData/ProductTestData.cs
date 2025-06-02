using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData
{
    public static class ProductTestData
    {
        public static Product ValidProduct()
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = "Produto Teste",
                Price = 99.99m,
            };
        }

        public static Product InactiveProduct()
        {
            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Produto Inativo",
                Price = 50m,
            };
            return p;
        }
    }
}
