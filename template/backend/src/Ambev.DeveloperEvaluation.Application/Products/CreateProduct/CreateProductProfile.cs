using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Configura o AutoMapper para mapear Product → CreateProductResult.
    /// </summary>
    public class CreateProductProfile : Profile
    {
        public CreateProductProfile()
        {
            CreateMap<Product, CreateProductResult>()
                .ForCtorParam(nameof(CreateProductResult.Id), opt => opt.MapFrom(p => p.Id))
                .ForCtorParam(nameof(CreateProductResult.Name), opt => opt.MapFrom(p => p.Name))
                .ForCtorParam(nameof(CreateProductResult.Price), opt => opt.MapFrom(p => p.Price));
        }
    }
}
