using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    /// <summary>
    /// Handler que processa GetProductQuery e retorna GetProductResult.
    /// </summary>
    public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResult>
    {
        private readonly DefaultContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductHandler> _logger;

        public GetProductHandler(DefaultContext db, IMapper mapper, ILogger<GetProductHandler> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetProductResult> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando produto: ProductId={ProductId}", request.Id);

            var product = await _db.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado: ProductId={ProductId}", request.Id);
                throw new KeyNotFoundException($"Produto não encontrado: {request.Id}");
            }

            return _mapper.Map<GetProductResult>(product);
        }
    }
}
