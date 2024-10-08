using CartService.Application.UseCases.CartItems.Commands;
using FluentValidation;

namespace CartService.Application.UseCases.CartItems.Validators
{
	public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
	{
		public AddItemToCartCommandValidator()
		{
			RuleFor(v => v.Item)
				.NotNull()
				.WithMessage("Cart Item must not be null.");

			When(v => v.Item != null, () =>
			{
				RuleFor(v => v.Item.Name)
					.NotNull()
					.NotEmpty()
					.WithMessage("Name is required.");

				RuleFor(v => v.Item.Price)
					.GreaterThan(0)
					.WithMessage("Price must not be a positive number.");

				RuleFor(v => v.Item.Quantity)
					.GreaterThan(0)
					.WithMessage("Quantity must not be a positive number.");
			});
		}
    }
}
