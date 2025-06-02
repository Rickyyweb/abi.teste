namespace Ambev.DeveloperEvaluation.Application.Interfaces
{
    /// <summary>
    /// Interface para envio de e-mail. 
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>Envia um e-mail para o destinatário informado.</summary>
        Task SendEmailAsync(string to, string subject, string body);
    }
}
