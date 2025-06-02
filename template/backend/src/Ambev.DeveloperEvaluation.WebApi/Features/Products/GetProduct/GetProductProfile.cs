using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct
{
    public class GetProductProfile : Profile
    {
        public GetProductProfile()
        {
            CreateMap<Guid, GetProductQuery>()
                .ConstructUsing(id => new GetProductQuery(id));

            CreateMap<GetProductResult, GetProductResponse>();
        }
    }
}
