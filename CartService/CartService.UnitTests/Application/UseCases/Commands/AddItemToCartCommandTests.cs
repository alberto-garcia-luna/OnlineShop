﻿using CartService.Application.UseCases.CartItems.Commands;
using CartService.Domain.Entities;
using CartService.UnitTests.Application.Common;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace CartService.UnitTests.Application.UseCases.Commands
{
	[TestFixture]
	public class AddItemToCartCommandHandlerTests : TestBase
	{
		[SetUp]
		public void Setup()
		{
			_mockRepository.Reset();
		}

		[Test]
		public async Task Handle_ShouldAddItemToCart_WhenItemIsValid()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = ValidCartId,
				Id = 0,
				Name = "NewItem",
				Price = 15.99m,
				Quantity = 1
			};

			var command = new AddItemToCartCommand { Item = cartItem };

			_mockRepository.Setup(repo => repo.AddItemToCart(cartItem))
				.ReturnsAsync(1); // Simulate returning the ID of the added item

			// Act
			var result = await _mediator.Send(command);

			// Assert
			result.Should().Be(1);
			_mockRepository.Verify(repo => repo.AddItemToCart(cartItem), Times.Once);
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenItemIsInvalid()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = ValidCartId,
				Id = 0,
				Name = null, // Invalid because Name is required
				Price = 0m, // Invalid because Price must be positive
				Quantity = 0 // Invalid because Quantity must be positive
			};

			var command = new AddItemToCartCommand { Item = cartItem };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>();
			_mockRepository.Verify(repo => repo.AddItemToCart(It.IsAny<CartItem>()), Times.Never);
		}

		[Test]
		public async Task Handle_ShouldCallRepositoryWithCorrectItem()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = ValidCartId,
				Id = 0,
				Name = "AnotherItem",
				Price = 25.00m,
				Quantity = 2
			};

			var command = new AddItemToCartCommand { Item = cartItem };

			// Act
			await _mediator.Send(command);

			// Assert
			_mockRepository.Verify(repo => repo.AddItemToCart(cartItem), Times.Once);
		}
	}
}