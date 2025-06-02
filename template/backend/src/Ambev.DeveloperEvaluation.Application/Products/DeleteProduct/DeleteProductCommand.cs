using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct
{
    /// <summary>
    /// Command para remover um produto por Id.
    /// </summary>
    public record DeleteProductCommand(Guid Id) : IRequest<DeleteProductResult>;
}
