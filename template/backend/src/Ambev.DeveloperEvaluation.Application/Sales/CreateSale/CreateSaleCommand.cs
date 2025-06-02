using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public record CreateSaleItemDto(
        Guid ProductId,
        int Quantity
    );

    public record CreateSaleCommand(
        string IdempotencyKey,               
        Guid CustomerId,
        List<CreateSaleItemDto> Items
    ) : IRequest<CreateSaleResult>;
}
