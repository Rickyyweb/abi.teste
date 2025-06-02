namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Resultado retornado após criar um produto.
    /// </summary>
    public record CreateProductResult(
        Guid Id,
        string Name,
        decimal Price
    );
}
