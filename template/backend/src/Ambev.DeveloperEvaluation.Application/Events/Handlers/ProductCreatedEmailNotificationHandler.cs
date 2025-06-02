using Ambev.DeveloperEvaluation.Application.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events.Product;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    /// <summary>
    /// Handler que é chamado sempre que ProductCreatedEvent é publicado.
    /// Simula o envio de e-mail para um “comprador” ou “stakeholder”.
    /// </summary>
    public class ProductCreatedEmailNotificationHandler : INotificationHandler<ProductCreatedEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ProductCreatedEmailNotificationHandler> _logger;

        public ProductCreatedEmailNotificationHandler(
            IEmailSender emailSender,
            ILogger<ProductCreatedEmailNotificationHandler> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Simula um e-mail para ilustrar a reação:
            var subject = $"Novo produto cadastrado: {notification.ProductId}";
            var body = $"O produto com ID {notification.ProductId} foi registrado com sucesso no sistema.";

            // Envia e-mail (simulado)
            await _emailSender.SendEmailAsync("comprador@exemplo.com", subject, body);

            _logger.LogInformation(
                "ProductCreatedEmailNotificationHandler: Email enviado para comprador sobre ProductId={ProductId}",
                notification.ProductId);
        }
    }
}
