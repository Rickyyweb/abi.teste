using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Representa uma venda (invoice) feita por um cliente.
    /// Encapsula toda a lógica de negócio: limite de itens iguais, cálculo de subtotal, desconto e total.
    /// </summary>
    public class Sale : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalAmount { get; private set; }

        // Backing field para os itens de venda
        private readonly List<SaleItem> _items = new();
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

        /// <summary>
        /// Construtor protegido para EF Core.
        /// </summary>
        protected Sale() { }

        /// <summary>
        /// Inicia uma nova venda para o cliente informado.
        /// </summary>
        public Sale(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            CreatedAt = DateTime.UtcNow;

            Subtotal = 0m;
            Discount = 0m;
            TotalAmount = 0m;
        }

        /// <summary>
        /// Adiciona um item a esta venda.  
        /// Se já houver itens do mesmo produto, verifica o limite de 20 unidades.
        /// </summary>
        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException("Quantidade deve ser maior que zero.");

            if (unitPrice <= 0m)
                throw new DomainException("UnitPrice deve ser maior que zero.");

            // Verifica quantas unidades deste produto já existem
            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            var existingQuantity = existingItem?.Quantity ?? 0;

            if (existingQuantity + quantity > 20)
                throw new DomainException("Um pedido não pode conter mais de 20 unidades do mesmo item.");

            // Se já existe o item, só acumula a quantidade e recalcula
            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                // Cria novo SaleItem
                var item = new SaleItem(productId, quantity, unitPrice);
                _items.Add(item);
            }

            // Recalcula totais e desconto
            RecalculateTotals();
        }

        public void UpdateItem(Guid productId, int newQuantity, decimal unitPrice)
        {
            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem == null)
                throw new DomainException($"Não existe item com ProductId {productId} para atualizar.");

            if (newQuantity < 0)
                throw new DomainException("Quantidade não pode ser negativa.");

            // Se a nova quantidade for zero, apenas remove
            if (newQuantity == 0)
            {
                _items.Remove(existingItem);
                RecalculateTotals();
                return;
            }

            // Atualiza diretamente as propriedades do SaleItem existente
            existingItem.UpdateQuantity(newQuantity, unitPrice);

            // Recalcula subtotal/discount/total
            RecalculateTotals();
        }


        /// <summary>
        /// Remove todos os itens da venda (caso queira recomeçar ou excluir).
        /// </summary>
        public void RemoveAllItems()
        {
            _items.Clear();
            RecalculateTotals();
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
                _items.Remove(item);

            RecalculateTotals();
        }

        /// <summary>
        /// Recalcula Subtotal, Discount e TotalAmount toda vez que a lista de itens muda.
        /// </summary>
        private void RecalculateTotals()
        {
            Subtotal = _items.Sum(i => i.Quantity * i.UnitPrice);

            var totalQuantity = _items.Sum(i => i.Quantity);

            decimal percentage = 0m;

            if (totalQuantity >= 20)
                percentage = 0.20m;   // 20%
            else if (totalQuantity >= 10)
                percentage = 0.10m;   // 10%

            Discount = Math.Round(Subtotal * percentage, 2, MidpointRounding.AwayFromZero);

            TotalAmount = Subtotal - Discount;
        }
    }
}
