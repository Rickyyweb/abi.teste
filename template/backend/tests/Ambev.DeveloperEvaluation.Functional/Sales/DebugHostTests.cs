using Ambev.DeveloperEvaluation.Functional.CustomWebApplicationFactory;
using Ambev.DeveloperEvaluation.WebApi;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales
{
    public class DebugHostTests
    {
        [Fact]
        public void DebugHostInitialization()
        {
            var factory = new CustomWebApplicationFactory<Program>();
            try
            {
                // Ao chamar CreateClient(), o factory tentará construir o IHost internamente.
                // Se houver um problema (por ex. não encontrar appsettings.json), ele lança InvalidOperationException.
                var client = factory.CreateClient();
            }
            catch (InvalidOperationException ex)
            {
                // Aqui, imprimimos a InnerException (se houver) para o console de testes:
                Console.WriteLine("=== DebugHostTests: InnerException começou abaixo ===");
                Console.WriteLine(ex.InnerException?.ToString() ?? "(Nenhuma InnerException vazia)");
                Console.WriteLine("=== DebugHostTests: InnerException terminou acima ===");

                // Se você quiser enxergar a mensagem sem falhar o teste, comente a linha abaixo.
                // Caso contrário, descomente para que o teste falhe e você veja o stack trace.
                // Assert.Null(ex.InnerException);
            }
        }
    }
}
