﻿using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    public class DeleteSaleProfile : Profile
    {
        public DeleteSaleProfile()
        {
            CreateMap<Guid, DeleteSaleCommand>()
                .ConstructUsing(id => new DeleteSaleCommand(id));

            CreateMap<DeleteSaleResult, DeleteSaleResponse>();
        }
    }
}
