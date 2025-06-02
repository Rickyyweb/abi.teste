using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleProfile : Profile
    {
        public UpdateSaleProfile()
        {
            CreateMap<UpdateSaleItemDto, Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleItemDto>();

            // 2) Mapeia o request inteiro → UpdateSaleCommand
            CreateMap<UpdateSaleRequest, Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleCommand>()
                // IdempotencyKey vem do corpo
                .ForCtorParam(
                    nameof(Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleCommand.IdempotencyKey),
                    opt => opt.MapFrom(src => src.IdempotencyKey))
                // SaleId deixaremos padrão aqui (Guid.Empty) e sobrescreveremos no controller
                .ForCtorParam(
                    nameof(Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleCommand.SaleId),
                    opt => opt.MapFrom(_ => Guid.Empty))
                // Itens → lista de Application.UpdateSaleItemDto
                .ForCtorParam(
                    nameof(Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleCommand.Items),
                    opt => opt.MapFrom(src => src.Items));

            // 3) Mapeia o resultado do handler → resposta WebApi
            CreateMap<Ambev.DeveloperEvaluation.Application.Sales.UpdateSale.UpdateSaleResult, UpdateSaleResponse>();

        }
    }
}
