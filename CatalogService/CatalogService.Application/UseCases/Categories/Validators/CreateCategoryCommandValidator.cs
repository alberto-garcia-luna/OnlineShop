using CatalogService.Application.UseCases.Categories.Commands;
using FluentValidation;

namespace CatalogService.Application.UseCases.Categories.Validators
{
	public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommnad>
	{
		public CreateCategoryCommandValidator()
        {
			RuleFor(v => v.Category)
				.NotNull()
				.WithMessage("Category must not be null.");

			When(v => v.Category != null, () =>
			{
				RuleFor(v => v.Category.Name)
					.NotNull() // Ensure Category is not null before checking Name
					.NotEmpty()
					.MaximumLength(50)
					.WithMessage("Name is required.");
			});
		}
    }
}