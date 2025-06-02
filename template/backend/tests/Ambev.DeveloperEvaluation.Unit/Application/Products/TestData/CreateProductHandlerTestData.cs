using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData
{
    public static class CreateProductHandlerTestData
    {
        public static CreateProductCommand ValidCommand()
        {
            return new CreateProductCommand(
                Name: "Produto de Teste",
                Price: 123.45m
            );
        }

        public static CreateProductCommand InvalidCommand_MissingName()
        {
            return new CreateProductCommand(
                Name: string.Empty, // inválido
                Price: 50m
            );
        }
    }
}
