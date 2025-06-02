using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Command para atualizar um produto existente.
    /// </summary>
    public record UpdateProductCommand(Guid Id, string Name, decimal Price) : IRequest<UpdateProductResult>;

}
