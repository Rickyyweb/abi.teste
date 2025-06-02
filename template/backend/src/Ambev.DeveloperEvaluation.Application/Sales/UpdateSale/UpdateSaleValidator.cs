using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty().WithMessage("SaleId é obrigatório.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Lista de itens é obrigatória.")
                .Must(items => items.Count > 0)
                    .WithMessage("Deve existir pelo menos um item na venda.");

            RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemValidator());
        }
    }

    /// <summary>
    /// Validator para cada item na atualização de venda.
    /// </summary>
    public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemDto>
    {
        public UpdateSaleItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId é obrigatório.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity deve ser maior que zero.");
        }
    }
}
