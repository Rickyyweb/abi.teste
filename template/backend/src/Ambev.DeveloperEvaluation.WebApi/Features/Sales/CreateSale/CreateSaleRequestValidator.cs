using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
        {
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty().WithMessage("O campo 'IdempotencyKey' é obrigatório.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("O campo 'CustomerId' é obrigatório.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("A lista de itens é obrigatória.")
                .Must(list => list.Count > 0).WithMessage("Deve ter pelo menos um item na venda.");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateSaleItemDtoValidator());
        }
    }

    public class CreateSaleItemDtoValidator : AbstractValidator<CreateSaleItemDto>
    {
        public CreateSaleItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O campo 'ProductId' é obrigatório.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        }
    }
}
