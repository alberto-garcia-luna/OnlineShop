using Ardalis.GuardClauses;
using CartService.Application.UseCases.CartItems.Commands;
using CartService.Domain.Entities;
using CartService.UnitTests.Application.Common;
using FluentAssertions;
using Moq;

namespace CartService.UnitTests.Application.UseCases.Commands
{
	[TestFixture]
	public class RemoveItemFromCartCommandHandlerTests : TestBase
	{
		[Test]
		public async Task Handle_ShouldRemoveItemFromCart_WhenItemExists()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = 1
			};

			_mockRepository
				.Setup(repo => repo.GetCartItems(command.CartId))
				.ReturnsAsync(new List<CartItem>
				{
					new CartItem { CartId = ValidCartId, Id = 1, Name = "Item1", Price = 10.0m, Quantity = 1 }
				});

			_mockRepository
				.Setup(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId))
				.Returns(Task.CompletedTask);

			// Act
			await _mediator.Send(command);

			// Assert
			_mockRepository.Verify(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId), Times.Once);
		}

		[Test]
		public async Task Handle_ShouldThrowNotFoundException_WhenCartDoesNotExist()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = 1,
				ItemId = 1
			};

			_mockRepository
				.Setup(repo => repo.GetCartItems(command.CartId))
				.ReturnsAsync(new List<CartItem>());

			// Act
			Func<Task> action = async () => await _mediator.Send(command);

			// Assert
			await action.Should().ThrowAsync<NotFoundException>();
		}

		[Test]
		public async Task Handle_ShouldNotThrowException_WhenItemDoesNotExist()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = -1
			};

			_mockRepository
				.Setup(repo => repo.GetCartItems(command.CartId))
				.ReturnsAsync(new List<CartItem>
				{
					new CartItem { CartId = ValidCartId, Id = 1, Name = "Item1", Price = 10.0m, Quantity = 1 }
				});

			// Act
			Func<Task> action = async () => await _mediator.Send(command);

			// Assert
			await action.Should().ThrowAsync<NotFoundException>();
		}
	}
}