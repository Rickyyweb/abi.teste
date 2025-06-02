using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Command para criar um novo produto.
    /// </summary>
    public record CreateProductCommand(string Name, decimal Price) : IRequest<CreateProductResult>;

}
