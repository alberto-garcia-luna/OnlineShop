using CartService.Application.UseCases.CartItems.Commands;
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

			_mockRepository.Setup(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId))
			.Returns(Task.CompletedTask);

			// Act
			await _mediator.Send(command);

			// Assert
			_mockRepository.Verify(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId), Times.Once);
		}

		[Test]
		public async Task Handle_ShouldNotThrowException_WhenItemDoesNotExist()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = 999 // Assuming this ID does not exist
			};

			_mockRepository.Setup(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId))
				.Returns(Task.CompletedTask);

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().NotThrowAsync();
			_mockRepository.Verify(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId), Times.Once);
		}

		[Test]
		public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
		{
			// Arrange
			var command = new RemoveItemFromCartCommand
			{
				CartId = ValidCartId,
				ItemId = 2
			};

			// Act
			await _mediator.Send(command);

			// Assert
			_mockRepository.Verify(repo => repo.RemoveItemFromCart(command.CartId, command.ItemId), Times.Once);
		}
	}
}