using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    /// <summary>
    /// Query para buscar um produto por Id.
    /// </summary>
    public record GetProductQuery(Guid Id) : IRequest<GetProductResult>;
}
