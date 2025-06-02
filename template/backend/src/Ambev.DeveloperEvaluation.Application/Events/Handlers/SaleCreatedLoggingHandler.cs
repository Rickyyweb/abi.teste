using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    /// <summary>
    /// Handler que é chamado sempre que um SaleCreatedEvent é publicado.
    /// Apenas loga no console/arquivo que a venda foi criada.
    /// </summary>
    public class SaleCreatedLoggingHandler : INotificationHandler<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedLoggingHandler> _logger;

        public SaleCreatedLoggingHandler(ILogger<SaleCreatedLoggingHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "SaleCreatedLoggingHandler: Detected SaleCreatedEvent for SaleId={SaleId}",
                notification.SaleId);

            return Task.CompletedTask;
        }
    }
}
