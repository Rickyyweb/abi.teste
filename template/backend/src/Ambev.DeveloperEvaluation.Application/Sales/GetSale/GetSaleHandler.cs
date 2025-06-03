using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    /// <summary>
    /// Handler que processa GetSaleQuery e retorna GetSaleResult.
    /// </summary>
    public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
    {
        private readonly DefaultContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSaleHandler> _logger;

        public GetSaleHandler(DefaultContext db, IMapper mapper, ILogger<GetSaleHandler> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetSaleResult> Handle(GetSaleQuery command, CancellationToken cancellationToken)
        {
            var validator = new GetSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _logger.LogInformation("Buscando venda: SaleId={SaleId}", command.Id);

            var sale = await _db.Sales
                                .Include(s => s.Items).ThenInclude(i => i.Product)
                                .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

            if (sale == null)
            {
                _logger.LogWarning("Venda não encontrada: SaleId={SaleId}", command.Id);
                throw new KeyNotFoundException($"Venda não encontrada: {command.Id}");
            }

            var result = _mapper.Map<GetSaleResult>(sale);
            _logger.LogInformation("GetSaleHandler finalizado. SaleId={SaleId}", command.Id);
            return result;
        }
    }
}
