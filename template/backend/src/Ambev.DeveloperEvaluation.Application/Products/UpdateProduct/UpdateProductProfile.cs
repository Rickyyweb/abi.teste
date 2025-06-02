using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    public class UpdateProductProfile : Profile
    {
        public UpdateProductProfile()
        {
            // Aqui só precisamos retornar o Id, mas mantemos esse Profile por consistência.
            CreateMap<Product, UpdateProductResult>()
                .ForCtorParam(nameof(UpdateProductResult.Id), opt => opt.MapFrom(p => p.Id));
        }
    }
}
