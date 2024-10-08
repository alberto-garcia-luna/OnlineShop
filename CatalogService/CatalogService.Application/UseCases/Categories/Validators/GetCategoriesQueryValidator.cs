using CatalogService.Application.UseCases.Categories.Queries;
using FluentValidation;

namespace CatalogService.Application.UseCases.Categories.Validators
{
	public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
	{
        public GetCategoriesQueryValidator()
        {
			RuleFor(x => x.PageNumber)
				.GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

			RuleFor(x => x.PageSize)
				.GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
		}
    }
}
