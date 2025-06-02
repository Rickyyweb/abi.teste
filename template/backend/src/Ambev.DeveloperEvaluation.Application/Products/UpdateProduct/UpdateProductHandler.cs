using Ambev.DeveloperEvaluation.ORM.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Handler que processa UpdateProductCommand e retorna UpdateProductResult.
    /// </summary>
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
    {
        private readonly DefaultContext _db;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(DefaultContext db, ILogger<UpdateProductHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Atualizando produto. ProductId={ProductId}", request.Id);

            var product = await _db.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado para update. ProductId={ProductId}", request.Id);
                throw new KeyNotFoundException($"Produto não encontrado: {request.Id}");
            }

            product.Update(request.Name, request.Price);
            _db.Products.Update(product);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Produto atualizado com sucesso. ProductId={ProductId}", request.Id);
            return new UpdateProductResult(request.Id);
        }
    }
}
