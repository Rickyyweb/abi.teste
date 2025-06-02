using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Validator para UpdateProductCommand.
    /// </summary>
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name é obrigatório.")
                .MaximumLength(200).WithMessage("Name não pode exceder 200 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0m).WithMessage("Price não pode ser negativo.");
        }
    }
}
