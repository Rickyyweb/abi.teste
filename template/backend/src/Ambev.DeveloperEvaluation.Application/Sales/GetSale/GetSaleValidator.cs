using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    /// <summary>
    /// Validator para GetSaleQuery.
    /// </summary>
    public class GetSaleValidator : AbstractValidator<GetSaleQuery>
    {
        public GetSaleValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id da venda é obrigatório.");
        }
    }
}
