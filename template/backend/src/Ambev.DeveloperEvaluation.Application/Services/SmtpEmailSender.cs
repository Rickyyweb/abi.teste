using Ambev.DeveloperEvaluation.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ambev.DeveloperEvaluation.Application.Services
{
    /// <summary>
    /// Implementação de IEmailSender usando MailKit para envio real de e-mail via SMTP.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailSender(
            IOptions<SmtpSettings> smtpOptions,
            ILogger<SmtpEmailSender> logger)
        {
            _smtpSettings = smtpOptions.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Monta a mensagem MIME
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            // Vamos mandar texto simples; se quiser HTML, use TextFormat.Html
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new SmtpClient();
            try
            {
                // Se o servidor exigir STARTTLS, podemos usar SecureSocketOptions.StartTls ou Auto
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);

                if (!string.IsNullOrWhiteSpace(_smtpSettings.Username))
                {
                    await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("E-mail enviado com sucesso para {To}", to);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar e-mail para {To}", to);
                // Opcional: relance ou não, dependendo do comportamento desejado
                throw;
            }
        }
    }
}
