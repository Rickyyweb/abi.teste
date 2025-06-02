using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Validator para CreateSaleCommand.
    /// </summary>
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId é obrigatório.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Lista de itens é obrigatória.")
                .Must(items => items.Count > 0)
                    .WithMessage("Deve existir pelo menos um item na venda.");

            RuleForEach(x => x.Items).SetValidator(new CreateSaleItemValidator());
        }
    }

    /// <summary>
    /// Validator para cada item na venda.
    /// </summary>
    public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemDto>
    {
        public CreateSaleItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId é obrigatório.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity deve ser maior que zero.");
        }
    }
}
