using Ambev.DeveloperEvaluation.ORM.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// Handler que processa DeleteSaleCommand e retorna DeleteSaleResult.
    /// </summary>
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
    {
        private readonly DefaultContext _db;
        private readonly ILogger<DeleteSaleHandler> _logger;

        public DeleteSaleHandler(DefaultContext db, ILogger<DeleteSaleHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DeleteSaleResult> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removendo venda: SaleId={SaleId}", request.SaleId);

            // Carrega a venda incluindo itens (para que a exclusão em cascade funcione)
            var sale = await _db.Sales
                                .Include(s => s.Items)
                                .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

            if (sale == null)
            {
                _logger.LogWarning("Venda não encontrada para remoção: SaleId={SaleId}", request.SaleId);
                throw new KeyNotFoundException($"Venda não encontrada: {request.SaleId}");
            }

            _db.Sales.Remove(sale);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venda removida com sucesso: SaleId={SaleId}", request.SaleId);
            return new DeleteSaleResult(request.SaleId);
        }
    }
}
