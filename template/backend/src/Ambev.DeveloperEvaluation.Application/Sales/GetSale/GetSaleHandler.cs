using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<GetSaleResult> Handle(GetSaleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando venda: SaleId={SaleId}", request.Id);

            var sale = await _db.Sales
                                .Include(s => s.Items).ThenInclude(i => i.Product)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sale == null)
            {
                _logger.LogWarning("Venda não encontrada: SaleId={SaleId}", request.Id);
                throw new KeyNotFoundException($"Venda não encontrada: {request.Id}");
            }

            var result = _mapper.Map<GetSaleResult>(sale);
            _logger.LogInformation("GetSaleHandler finalizado. SaleId={SaleId}", request.Id);
            return result;
        }
    }
}
