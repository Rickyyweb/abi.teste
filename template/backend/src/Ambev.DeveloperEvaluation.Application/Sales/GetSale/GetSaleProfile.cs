using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleProfile : Profile
    {
        public GetSaleProfile()
        {
            // SaleItem → GetSaleItemResult
            CreateMap<SaleItem, GetSaleItemResult>()
                .ForCtorParam(nameof(GetSaleItemResult.ProductId), opt => opt.MapFrom(i => i.ProductId))
                .ForCtorParam(nameof(GetSaleItemResult.ProductName), opt => opt.MapFrom(i => i.Product.Name))
                .ForCtorParam(nameof(GetSaleItemResult.ProductPrice), opt => opt.MapFrom(i => i.Product.Price))
                .ForCtorParam(nameof(GetSaleItemResult.Quantity), opt => opt.MapFrom(i => i.Quantity))
                .ForCtorParam(nameof(GetSaleItemResult.UnitPrice), opt => opt.MapFrom(i => i.UnitPrice))
                .ForCtorParam(nameof(GetSaleItemResult.TotalPrice), opt => opt.MapFrom(i => i.Quantity * i.UnitPrice));

            // Sale → GetSaleResult
            CreateMap<Sale, GetSaleResult>()
                .ForCtorParam(nameof(GetSaleResult.Id), opt => opt.MapFrom(s => s.Id))
                .ForCtorParam(nameof(GetSaleResult.Subtotal), opt => opt.MapFrom(s => s.Subtotal))
                .ForCtorParam(nameof(GetSaleResult.Discount), opt => opt.MapFrom(s => s.Discount))
                .ForCtorParam(nameof(GetSaleResult.TotalAmount), opt => opt.MapFrom(s => s.TotalAmount))
                .ForCtorParam(nameof(GetSaleResult.Items), opt => opt.MapFrom(s => s.Items));
        }
    }
}
