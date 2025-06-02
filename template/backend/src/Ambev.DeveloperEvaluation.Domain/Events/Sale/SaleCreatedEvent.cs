using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sale
{
    public class SaleCreatedEvent : INotification
    {
        /// <summary>Identificador da Venda recém‐criada.</summary>
        public Guid SaleId { get; }

        public SaleCreatedEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
