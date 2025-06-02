namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public record GetSaleItemResult(
        Guid ProductId,
        string ProductName,
        decimal ProductPrice,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
