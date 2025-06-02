namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleResponse
    {
        public CreateSaleResponse(
            Guid Id,
            decimal Subtotal,
            decimal Discount,
            decimal TotalAmount,
            List<CreateSaleItemResult> Items)
        {
            this.Id = Id;
            this.Subtotal = Subtotal;
            this.Discount = Discount;
            this.TotalAmount = TotalAmount;
            this.Items = Items;
        }

        public Guid Id { get; }
        public decimal Subtotal { get; }
        public decimal Discount { get; }
        public decimal TotalAmount { get; }
        public List<CreateSaleItemResult> Items { get; }


        public class CreateSaleItemResult
        {
            public CreateSaleItemResult(
                Guid ProductId,
                string ProductName,
                decimal ProductPrice,
                int Quantity,
                decimal UnitPrice,
                decimal TotalPrice)
            {
                this.ProductId = ProductId;
                this.ProductName = ProductName;
                this.ProductPrice = ProductPrice;
                this.Quantity = Quantity;
                this.UnitPrice = UnitPrice;
                this.TotalPrice = TotalPrice;
            }

            public Guid ProductId { get; }
            public string ProductName { get; }
            public decimal ProductPrice { get; }
            public int Quantity { get; }
            public decimal UnitPrice { get; }
            public decimal TotalPrice { get; }
        }
    }
}