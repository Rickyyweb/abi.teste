namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public record CreateSaleItemResult(
        Guid ProductId,
        string ProductName,
        decimal ProductPrice,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
