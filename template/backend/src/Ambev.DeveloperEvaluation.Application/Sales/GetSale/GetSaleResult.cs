namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public record GetSaleResult(
        Guid Id,
        decimal Subtotal,
        decimal Discount,
        decimal TotalAmount,
        List<GetSaleItemResult> Items
    );
}
