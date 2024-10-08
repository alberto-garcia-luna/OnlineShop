using CatalogService.Application.UseCases.Products.Commands;
using FluentValidation;

namespace CatalogService.Application.UseCases.Products.Validators
{
	public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommnad>
	{
        public UpdateProductCommandValidator()
        {
			RuleFor(v => v.Product)
				.NotNull()
				.WithMessage("Product must not be null.");

			When(v => v.Product != null, () =>
			{
				RuleFor(v => v.Product.Name)
					.NotNull() // Ensure Product is not null before checking Name
					.NotEmpty()
					.MaximumLength(50)
					.WithMessage("Name is required.");

				RuleFor(v => v.Product.Category)
					.NotNull()
					.WithMessage("Category must not be null.");

				RuleFor(v => v.Product.Price)
					.GreaterThan(0)
					.WithMessage("Price must not be a positive number.");

				RuleFor(v => v.Product.Amount)
					.GreaterThan(0)
					.WithMessage("Amount must not be a positive number.");
			});

			When(v => v.Product != null && v.Product.Category != null, () =>
			{
				RuleFor(v => v.Product.Category.Id)
					.NotNull()
					.WithMessage("Category Id must not be null.");
			});
		}
    }
}
