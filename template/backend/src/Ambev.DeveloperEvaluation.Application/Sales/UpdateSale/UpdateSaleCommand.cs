using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public record UpdateSaleItemDto(
        Guid ProductId,
        int Quantity
    );

    public record UpdateSaleCommand(
        string IdempotencyKey,
        Guid SaleId,
        List<UpdateSaleItemDto> Items
    ) : IRequest<UpdateSaleResult>;
}
