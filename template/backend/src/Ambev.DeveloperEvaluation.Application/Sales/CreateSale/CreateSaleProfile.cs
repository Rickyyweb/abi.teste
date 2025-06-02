using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            CreateMap<SaleItem, CreateSaleItemResult>()
                .ForCtorParam(nameof(CreateSaleItemResult.ProductId), opt => opt.MapFrom(i => i.ProductId))
                .ForCtorParam(nameof(CreateSaleItemResult.ProductName), opt => opt.MapFrom(i => i.Product.Name))
                .ForCtorParam(nameof(CreateSaleItemResult.ProductPrice), opt => opt.MapFrom(i => i.Product.Price))
                .ForCtorParam(nameof(CreateSaleItemResult.Quantity), opt => opt.MapFrom(i => i.Quantity))
                .ForCtorParam(nameof(CreateSaleItemResult.UnitPrice), opt => opt.MapFrom(i => i.UnitPrice))
                .ForCtorParam(nameof(CreateSaleItemResult.TotalPrice), opt => opt.MapFrom(i => i.Quantity * i.UnitPrice));

            CreateMap<Sale, CreateSaleResult>()
                .ForCtorParam(nameof(CreateSaleResult.Id), opt => opt.MapFrom(s => s.Id))
                .ForCtorParam(nameof(CreateSaleResult.Subtotal), opt => opt.MapFrom(s => s.Subtotal))
                .ForCtorParam(nameof(CreateSaleResult.Discount), opt => opt.MapFrom(s => s.Discount))
                .ForCtorParam(nameof(CreateSaleResult.TotalAmount), opt => opt.MapFrom(s => s.TotalAmount))
                .ForCtorParam(nameof(CreateSaleResult.Items), opt => opt.MapFrom(s => s.Items));
        }
    }
}
