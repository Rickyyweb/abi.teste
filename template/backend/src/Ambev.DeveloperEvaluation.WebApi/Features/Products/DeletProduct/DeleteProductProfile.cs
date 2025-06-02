using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct
{
 
    public class DeleteProductProfile : Profile
    {
        public DeleteProductProfile()
        {
            CreateMap<Guid, DeleteProductCommand>()
                .ConstructUsing(id => new DeleteProductCommand(id));

            CreateMap<DeleteProductResult, DeleteProductResult>();
        }
    }
}
