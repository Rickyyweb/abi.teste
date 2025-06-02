namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// DTO de resposta após remover uma venda (retorna apenas o SaleId).
    /// </summary>
    public record DeleteSaleResult(Guid SaleId);
}
