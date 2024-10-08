using CatalogService.Application.UseCases.Products.Queries;
using FluentValidation;

namespace CatalogService.Application.UseCases.Products.Validators
{
	public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
	{
        public GetProductsQueryValidator()
        {
			RuleFor(x => x.PageNumber)
				.GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

			RuleFor(x => x.PageSize)
				.GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
		}
    }
}
