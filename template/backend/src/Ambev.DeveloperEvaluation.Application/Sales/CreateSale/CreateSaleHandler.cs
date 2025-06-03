using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly DefaultContext _db;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSaleHandler> _logger;

        public CreateSaleHandler(
            DefaultContext db,
            IMediator mediator,
            IMapper mapper,
            ILogger<CreateSaleHandler> logger)
        {
            _db = db;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
        {

            var validator = new CreateSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // 1) Se já existe a chave de idempotência, retorna o resultado mapeado (sem recriar a sale)
            var existingKey = await _db.IdempotencyKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Key == command.IdempotencyKey, cancellationToken);

            if (existingKey != null)
            {
                _logger.LogInformation(
                    "CreateSaleHandler: IdempotencyKey {Key} já processada. Retornando SaleId existente {SaleId}.",
                    command.IdempotencyKey, existingKey.SaleId);

                var existingSale = await _db.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)  
                .FirstOrDefaultAsync(s => s.Id == existingKey.SaleId, cancellationToken);

                return _mapper.Map<CreateSaleResult>(existingSale);
            }

            _logger.LogInformation(
                "Iniciando criação de venda idempotente. IdempotencyKey={Key}, CustomerId={CustomerId}, QtdItems={QtdItems}",
                command.IdempotencyKey, command.CustomerId, command.Items.Count);

            // 2) Cria o aggregate
            var sale = new Sale(command.CustomerId);

            foreach (var dto in command.Items)
            {
                // Para cada item, busca o produto e obtém o preço
                var product = await _db.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId, cancellationToken);

                if (product == null)
                {
                    _logger.LogWarning(
                        "Produto não encontrado ao criar venda (idempotente). ProductId={ProductId}",
                        dto.ProductId);
                    throw new KeyNotFoundException($"Produto não encontrado: {dto.ProductId}");
                }

                _logger.LogDebug(
                    "  → Adicionando item: ProductId={ProductId}, Quantity={Quantity}, UnitPrice={UnitPrice}",
                    dto.ProductId, dto.Quantity, product.Price);

                sale.AddItem(dto.ProductId, dto.Quantity, product.Price);
            }

            // 3) Persiste Sale e registra a chave de idempotência
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            _db.Sales.Add(sale);
            await _db.SaveChangesAsync(cancellationToken);

            var keyEntry = new IdempotencyKeyEntry
            {
                Key = command.IdempotencyKey,
                SaleId = sale.Id
            };
            _db.IdempotencyKeys.Add(keyEntry);
            await _db.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Venda persistida no banco (idempotente). SaleId={SaleId}, Subtotal={Subtotal}, Discount={Discount}, TotalAmount={TotalAmount}",
                sale.Id, sale.Subtotal, sale.Discount, sale.TotalAmount);

            await _mediator.Publish(new SaleCreatedEvent(sale.Id), cancellationToken);
            _logger.LogInformation("SaleCreatedEvent publicado. SaleId={SaleId}", sale.Id);

            return _mapper.Map<CreateSaleResult>(sale);
        }
    }
}
