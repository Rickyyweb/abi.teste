using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Representa um item de venda: associação entre Venda e Produto, com quantidade e preço unitário.
    /// Contém validações para garantir que quantidade e unitPrice sejam válidos.
    /// </summary>
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        // Propriedades de navegação (EF Core)
        public Product Product { get; private set; }
        public Sale Sale { get; private set; }

        protected SaleItem() { }

        public void IncreaseQuantity(int moreQuantity)
        {
            if (moreQuantity <= 0)
                throw new DomainException("A quantidade a ser adicionada deve ser maior que zero.");

            Quantity += moreQuantity;
        }

        public SaleItem(Guid productId, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void UpdateQuantity(int newQuantity, decimal newUnitPrice)
        {
            if (newQuantity <= 0)
                throw new DomainException("Quantidade deve ser maior que zero.");

            if (newUnitPrice <= 0)
                throw new DomainException("UnitPrice deve ser maior que zero.");

            if (newQuantity > 20)
                throw new DomainException("Um pedido não pode conter mais de 20 unidades do mesmo item.");

            Quantity = newQuantity;
            UnitPrice = newUnitPrice;
        }
    }
}
