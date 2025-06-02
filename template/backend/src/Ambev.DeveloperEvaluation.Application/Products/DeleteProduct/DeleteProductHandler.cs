using Ambev.DeveloperEvaluation.ORM.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct
{
    /// <summary>
    /// Handler que processa DeleteProductCommand e retorna DeleteProductResponse.
    /// </summary>
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResult>
    {
        private readonly DefaultContext _db;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(DefaultContext db, ILogger<DeleteProductHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DeleteProductResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removendo produto: ProductId={ProductId}", request.Id);

            var product = await _db.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Produto não encontrado para remoção: ProductId={ProductId}", request.Id);
                throw new KeyNotFoundException($"Produto não encontrado: {request.Id}");
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Produto removido com sucesso: ProductId={ProductId}", request.Id);
            return new DeleteProductResult(request.Id);
        }
    }
}
