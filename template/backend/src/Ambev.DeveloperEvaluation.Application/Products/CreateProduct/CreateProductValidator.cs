using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Validator para CreateProductCommand usando FluentValidation.
    /// </summary>
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name é obrigatório.")
                .MaximumLength(200).WithMessage("Name não pode exceder 200 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0m).WithMessage("Price não pode ser negativo.");
        }
    }
}
