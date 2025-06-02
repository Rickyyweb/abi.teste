namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public record CreateSaleResult(
        Guid Id,
        decimal Subtotal,
        decimal Discount,
        decimal TotalAmount,
        List<CreateSaleItemResult> Items
    );
}
