using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Product;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Handler que processa CreateProductCommand e retorna CreateProductResult.
    /// </summary>
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
    {
        private readonly DefaultContext _db;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(DefaultContext db, IMediator mediator, IMapper mapper, ILogger<CreateProductHandler> logger)
        {
            _db = db;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Criando novo produto: {Name} com Price={Price}", request.Name, request.Price);

            var product = new Product(request.Name, request.Price);
            _db.Products.Add(product);
            await _db.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new ProductCreatedEvent(product.Id), cancellationToken);

            _logger.LogInformation("Produto criado: ProductId={ProductId}", product.Id);
            return _mapper.Map<CreateProductResult>(product);
        }
    }
}
