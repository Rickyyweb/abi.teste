using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
    {
        public UpdateSaleRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotNull().WithMessage("A lista de itens é obrigatória.")
                .Must(list => list.Count > 0).WithMessage("Deve ter pelo menos um item na venda.");

            RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemDtoValidator());
        }
    }

    public class UpdateSaleItemDtoValidator : AbstractValidator<UpdateSaleItemDto>
    {
        public UpdateSaleItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O campo 'ProductId' é obrigatório.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        }
    }
}
