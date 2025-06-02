using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation.Product
{
    public class ProductValidator : AbstractValidator<Entities.Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O campo 'Name' é obrigatório.");

            RuleFor(x => x.Price)
                .GreaterThan(0m)
                .WithMessage("O campo 'Price' deve ser maior que zero.");
        }
    }
}
