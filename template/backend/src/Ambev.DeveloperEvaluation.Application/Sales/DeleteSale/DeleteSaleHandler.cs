using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new DeleteSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _logger.LogInformation("Removendo venda: SaleId={SaleId}", command.SaleId);

            // Carrega a venda incluindo itens (para que a exclusão em cascade funcione)
            var sale = await _db.Sales
                                .Include(s => s.Items)
                                .FirstOrDefaultAsync(s => s.Id == command.SaleId, cancellationToken);

            if (sale == null)
            {
                _logger.LogInformation("Venda não encontrada para remoção: SaleId={SaleId}", command.SaleId);
                throw new KeyNotFoundException($"Venda não encontrada: {command.SaleId}");
            }

            _db.Sales.Remove(sale);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venda removida com sucesso: SaleId={SaleId}", command.SaleId);
            return new DeleteSaleResult(command.SaleId);
        }
    }
}
