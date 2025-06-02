using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    /// <summary>
    /// Validator para GetProductQuery.
    /// </summary>
    public class GetProductValidator : AbstractValidator<GetProductQuery>
    {
        public GetProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id é obrigatório.");
        }
    }
}
