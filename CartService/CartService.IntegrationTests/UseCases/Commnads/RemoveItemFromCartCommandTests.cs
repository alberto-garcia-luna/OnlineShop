using CartService.Application.UseCases.CartItems.Commands;
using CartService.Domain.Entities;
using CartService.IntegrationTests.Common;
using FluentAssertions;

namespace CartService.IntegrationTests.UseCases.Commnads
{
	[TestFixture]
	public class RemoveItemFromCartCommandTests : TestBase
	{
		[Test]
		public async Task Handle_ShouldRemoveItemFromCart_WhenItemExists()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = ValidCartId,
				Name = "ItemToRemove",
				Price = 10.00m,
				Quantity = 2
			};

			// Seed the database with the item to be removed
			var cartItemId = await _cartRepository.AddItemToCart(cartItem);

			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = cartItemId
			};

			// Act
			await _mediator.Send(command);

			// Assert
			var cartItems = await _cartRepository.GetCartItems(ValidCartId);
			cartItems.Should().BeEmpty(); // The cart should be empty after removal
		}

		[Test]
		public async Task Handle_ShouldNotThrow_WhenRemovingNonExistentItem()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = 9999 // Non-existent item ID
			};

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().NotThrowAsync(); // Ensure no exception is thrown
		}

		[Test]
		public async Task Handle_ShouldRemoveCorrectItem_WhenMultipleItemsInCart()
		{
			// Arrange
			var item1 = new CartItem
			{
				CartId = ValidCartId,
				Name = "Item1",
				Price = 10.00m,
				Quantity = 1
			};

			var item2 = new CartItem
			{
				CartId = ValidCartId,
				Name = "Item2",
				Price = 20.00m,
				Quantity = 2
			};

			await _cartRepository.AddItemToCart(item1);
			await _cartRepository.AddItemToCart(item2);

			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = item1.Id
			};

			// Act
			await _mediator.Send(command);

			// Assert
			var cartItems = await _cartRepository.GetCartItems(ValidCartId);
			cartItems.Should().ContainSingle()
				.Which.Should().BeEquivalentTo(item2, options => options.Excluding(x => x.Id)); // Only item2 should remain
		}
	}
}