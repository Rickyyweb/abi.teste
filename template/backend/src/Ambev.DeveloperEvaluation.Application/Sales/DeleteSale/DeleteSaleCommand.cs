using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// Command para remover uma venda existente pelo Id.
    /// </summary>
    public record DeleteSaleCommand(Guid SaleId) : IRequest<DeleteSaleResult>;

}
