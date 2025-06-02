using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly DefaultContext _db;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSaleHandler> _logger;

        public UpdateSaleHandler(
            DefaultContext db,
            IMediator mediator,
            ILogger<UpdateSaleHandler> logger)
        {
            _db = db;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _db.Sales
                                .Include(s => s.Items)
                                .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Venda não encontrada: {request.SaleId}");

            // 2) Verifica Idempotência
            var existingKey = await _db.IdempotencyKeys
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Key == request.IdempotencyKey, cancellationToken);
            if (existingKey != null)
            {
                _logger.LogInformation(
                    "UpdateSaleHandler: IdempotencyKey {Key} já processada. Retornando SaleId existente {SaleId}.",
                    request.IdempotencyKey, existingKey.SaleId);
                return new UpdateSaleResult(existingKey.SaleId);
            }

            var grouped = request.Items
                                 .GroupBy(i => i.ProductId)
                                 .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(i => i.Quantity) });
            if (grouped.Any(g => g.TotalQty > 20))
                throw new InvalidOperationException("Limite de 20 unidades do mesmo item excedido.");

            var requestProductIds = request.Items.Select(i => i.ProductId).ToHashSet();

            // 5) Percorre cada DTO do request e atualiza ou adiciona
            foreach (var dto in request.Items)
            {
                // Busca preço atual do produto (não rastreado)
                var product = await _db.Products
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(p => p.Id == dto.ProductId, cancellationToken);
                if (product == null)
                    throw new KeyNotFoundException($"Produto não encontrado: {dto.ProductId}");

                // Verifica se já existe aquele item na venda
                var existingItem = sale.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
                if (existingItem != null)
                {
                    sale.UpdateItem(dto.ProductId, dto.Quantity, product.Price);
                }
                else
                {
                    sale.AddItem(dto.ProductId, dto.Quantity, product.Price);
                }
            }

            var itensParaRemover = sale.Items
                                       .Where(i => !requestProductIds.Contains(i.ProductId))
                                       .Select(i => i.ProductId)
                                       .ToList();

            foreach (var prodId in itensParaRemover)
            {
                var itemParaDeletar = await _db.SaleItems
                                               .FirstOrDefaultAsync(i => i.SaleId == sale.Id
                                                                      && i.ProductId == prodId,
                                                                     cancellationToken);
                if (itemParaDeletar != null)
                {
                    _db.SaleItems.Remove(itemParaDeletar);
                    sale.RemoveItem(prodId);
                }
            }

            // 7) Cria registro de idempotência (ainda não salvo)
            var keyEntry = new IdempotencyKeyEntry
            {
                Key = request.IdempotencyKey,
                SaleId = sale.Id
            };
            _db.IdempotencyKeys.Add(keyEntry);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new KeyNotFoundException($"Venda não encontrada (concorrência): {request.SaleId}");
            }

            return new UpdateSaleResult(sale.Id);
        }
    }
}
