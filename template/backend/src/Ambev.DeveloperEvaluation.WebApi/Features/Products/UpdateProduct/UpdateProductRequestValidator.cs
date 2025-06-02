using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O campo 'Id' é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O campo 'Name' é obrigatório.")
                .MaximumLength(200).WithMessage("O 'Name' não pode exceder 200 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0m).WithMessage("O campo 'Price' não pode ser negativo.");
        }
    }
}
