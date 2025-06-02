namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleRequest
    {
        public string IdempotencyKey { get; set; }
        public List<UpdateSaleItemDto> Items { get; set; }
    }

    public class UpdateSaleItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
