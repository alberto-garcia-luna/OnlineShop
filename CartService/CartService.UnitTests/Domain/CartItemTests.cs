using CartService.Domain.Entities;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace CartService.UnitTests.Domain
{
	[TestFixture]
	public class CartItemTests
	{
		[Test]
		public void CartItem_ShouldBeValid_WhenAllRequiredFieldsAreFilled()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = "Laptop",
				Image = "http://example.com/image.jpg",
				Price = 999.99m,
				Quantity = 1
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void CartItem_ShouldBeInvalid_WhenNameIsNull()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = null,
				Image = "http://example.com/image.jpg",
				Price = 999.99m,
				Quantity = 1
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Name field is required.");
		}

		[Test]
		public void CartItem_ShouldBeInvalid_WhenPriceIsZero()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = "Laptop",
				Image = "http://example.com/image.jpg",
				Price = 0m,
				Quantity = 1
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("Price must be a positive value.");
		}

		[Test]
		public void CartItem_ShouldBeInvalid_WhenQuantityIsZero()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = "Laptop",
				Image = "http://example.com/image.jpg",
				Price = 999.99m,
				Quantity = 0
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("Quantity must be a positive integer.");
		}

		[Test]
		public void CartItem_ShouldBeValid_WhenImageIsNull()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = "Laptop",
				Image = null,
				Price = 999.99m,
				Quantity = 1
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void CartItem_ShouldBeInvalid_WhenImageIsInvalidUrl()
		{
			// Arrange
			var cartItem = new CartItem
			{
				CartId = "CartUniqueKey-1",
				Id = 1,
				Name = "Laptop",
				Image = "invalid-url",
				Price = 999.99m,
				Quantity = 1
			};

			// Act
			var validationResults = ValidateCartItem(cartItem);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Image field is not a valid fully-qualified http, https, or ftp URL.");
		}

		private IEnumerable<ValidationResult> ValidateCartItem(CartItem cartItem)
		{
			var context = new ValidationContext(cartItem);
			var results = new List<ValidationResult>();

			Validator.TryValidateObject(cartItem, context, results, true);
			return results;
		}
	}
}