namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    public record GetProductResult(
        Guid Id,
        string Name,
        decimal Price
    );
}
