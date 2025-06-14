﻿using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
    {
        public DeleteSaleCommandValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty().WithMessage("SaleId é obrigatório.");
        }
    }
}
