namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public record UpdateSaleItemResult(
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice
    );
}
