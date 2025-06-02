namespace Ambev.DeveloperEvaluation.Application.Services
{
    /// <summary>
    /// Configurações de SMTP que serão lidas do appsettings.json.
    /// </summary>
    public class SmtpSettings
    {
        public string Server { get; set; } = "";
        public int Port { get; set; }
        public string SenderName { get; set; } = "";
        public string SenderEmail { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; 
    }
}
