using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    /// <summary>
    /// Validator para GetSaleQuery.
    /// </summary>
    public class GetSaleCommandValidator : AbstractValidator<GetSaleQuery>
    {
        public GetSaleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id da venda é obrigatório.");
        }
    }
}
