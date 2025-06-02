using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Product
{
    /// <summary>
    /// Evento do domínio que representa que um novo Produto foi cadastrado.
    /// </summary>
    public class ProductCreatedEvent : INotification
    {
        /// <summary>Identificador do Produto recém‐cadastro.</summary>
        public Guid ProductId { get; }

        public ProductCreatedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}
