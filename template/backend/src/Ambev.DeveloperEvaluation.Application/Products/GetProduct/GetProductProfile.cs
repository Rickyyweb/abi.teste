using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    public class GetProductProfile : Profile
    {
        public GetProductProfile()
        {
            CreateMap<Product, GetProductResult>()
                .ForCtorParam(nameof(GetProductResult.Id), opt => opt.MapFrom(p => p.Id))
                .ForCtorParam(nameof(GetProductResult.Name), opt => opt.MapFrom(p => p.Name))
                .ForCtorParam(nameof(GetProductResult.Price), opt => opt.MapFrom(p => p.Price));
        }
    }
}
