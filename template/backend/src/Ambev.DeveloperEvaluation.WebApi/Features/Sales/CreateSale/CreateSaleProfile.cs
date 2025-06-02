using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            // 1) Mapeia cada item webapi → item application
            CreateMap<CreateSaleItemDto, Application.Sales.CreateSale.CreateSaleItemDto>()
                .ConstructUsing(src => new Application.Sales.CreateSale.CreateSaleItemDto(
                    src.ProductId,
                    src.Quantity
                ));

            // 2) Mapeia a requisição inteira (CreateSaleRequest) → CreateSaleCommand
            CreateMap<CreateSaleRequest, CreateSaleCommand>()
                .ForCtorParam(
                    nameof(CreateSaleCommand.IdempotencyKey),
                    opt => opt.MapFrom(src => src.IdempotencyKey)
                )
                .ForCtorParam(
                    nameof(CreateSaleCommand.CustomerId),
                    opt => opt.MapFrom(src => src.CustomerId)
                )
                .ForCtorParam(
                    nameof(CreateSaleCommand.Items),
                    opt => opt.MapFrom(src => src.Items)
                );


            CreateMap<CreateSaleResult, CreateSaleResponse>()
                .ForCtorParam(
                    nameof(CreateSaleResponse.Id),
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.Subtotal),
                    opt => opt.MapFrom(src => src.Subtotal)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.Discount),
                    opt => opt.MapFrom(src => src.Discount)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.TotalAmount),
                    opt => opt.MapFrom(src => src.TotalAmount)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.Items),
                    opt => opt.MapFrom(src => src.Items)
                );

            CreateMap<CreateSaleItemResult, CreateSaleResponse.CreateSaleItemResult>()
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.ProductId),
                    opt => opt.MapFrom(src => src.ProductId)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.ProductName),
                    opt => opt.MapFrom(src => src.ProductName)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.ProductPrice),
                    opt => opt.MapFrom(src => src.ProductPrice)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.Quantity),
                    opt => opt.MapFrom(src => src.Quantity)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.UnitPrice),
                    opt => opt.MapFrom(src => src.UnitPrice)
                )
                .ForCtorParam(
                    nameof(CreateSaleResponse.CreateSaleItemResult.TotalPrice),
                    opt => opt.MapFrom(src => src.TotalPrice)
                );
        }
    }
}
