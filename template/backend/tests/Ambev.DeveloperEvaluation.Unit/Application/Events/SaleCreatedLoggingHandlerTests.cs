using Ambev.DeveloperEvaluation.Application.Events.Handlers;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Events
{
    public class SaleCreatedLoggingHandlerTests
    {
        [Fact(DisplayName = "SaleCreatedLoggingHandler: Deve logar sem lançar exceção")]
        public void Handle_WhenCalled_LogsInformation()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SaleCreatedLoggingHandler>>();
            var handler = new SaleCreatedLoggingHandler(mockLogger.Object);
            var evt = new SaleCreatedEvent(Guid.NewGuid());

            // Act / Assert (não deve lançar)
            var ex = Record.ExceptionAsync(() => handler.Handle(evt, CancellationToken.None)).Result;
            ex.Should().BeNull();

            // (Opcional) verificar que o método LogInformation foi chamado com a mensagem apropriada:
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"SaleId={evt.SaleId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
