using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData
{
    public static class CreateSaleHandlerTestData
    {
        public static CreateSaleCommand ValidCommand(Guid productA, Guid productB)
        {
            return new CreateSaleCommand(
                IdempotencyKey: "abc123",
                CustomerId: Guid.NewGuid(),
                Items: new List<CreateSaleItemDto>  // Removido CreateSaleCommand.
                {
                    new CreateSaleItemDto(productA, 2),  // Usando CreateSaleItemDto diretamente
                    new CreateSaleItemDto(productB, 1)
                }
            );
        }

        public static CreateSaleCommand InvalidCommand_TooManySameItems(Guid productId)
        {
            var itens = new List<CreateSaleItemDto>();  // Removido CreateSaleCommand.
            for (int i = 0; i < 21; i++)
            {
                itens.Add(new CreateSaleItemDto(productId, 1));  // Usando CreateSaleItemDto diretamente
            }
            return new CreateSaleCommand(IdempotencyKey: "abc123", Guid.NewGuid(), itens);
        }
    }
}
