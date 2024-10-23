using CartService.Application.UseCases.CartItems.Commands;
using CartService.Application.UseCases.CartItems.Queries;
using CartService.IntegrationTests.Common;
using FluentAssertions;

namespace CartService.IntegrationTests.UseCases.Commnads
{
	[TestFixture]
	public class AddItemToCartCommandTests : TestBase
	{
		[Test]
		public async Task Handle_ShouldAddItemToCart_WhenItemIsValid()
		{
			// Arrange
			var cartItem = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "NewItem",
				Price = 15.00m,
				Quantity = 3
			};
			var command = new AddItemToCartCommand { Item = cartItem };

			// Act
			var result = await _mediator.Send(command);

			// Assert
			result.Should().BeGreaterThan(0); // Expecting a valid Id to be returned

			var cartItems = await _cartRepository.GetCartItems(ValidCartId);
			cartItems.Should().ContainSingle()
				.Which.Should().BeEquivalentTo(cartItem, options => options.Excluding(x => x.Id));
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenItemIsInvalid()
		{
			// Arrange
			var invalidItem = new CartItemDto
			{
				CartId = ValidCartId,
				Name = string.Empty, // Invalid name
				Price = -5.00m, // Invalid price
				Quantity = 0 // Invalid quantity
			};

			var command = new AddItemToCartCommand { Item = invalidItem };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<FluentValidation.ValidationException>();
		}

		[Test]
		public async Task Handle_ShouldAddMultipleItemsToCart_WhenItemsAreValid()
		{
			// Arrange
			var item1 = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "Item1",
				Price = 10.00m,
				Quantity = 1
			};

			var item2 = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "Item2",
				Price = 20.00m,
				Quantity = 2
			};

			var command1 = new AddItemToCartCommand { Item = item1 };
			var command2 = new AddItemToCartCommand { Item = item2 };

			// Act
			await _mediator.Send(command1);
			await _mediator.Send(command2);

			// Assert
			var cartItems = await _cartRepository.GetCartItems(ValidCartId);
			cartItems.Should().HaveCount(2);
		}
	}
}