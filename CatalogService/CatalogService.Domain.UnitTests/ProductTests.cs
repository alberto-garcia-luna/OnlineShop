using CatalogService.Domain.Entities;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.UnitTests
{
	[TestFixture]
	internal class ProductTests
	{
		[Test]
		public void Product_ShouldBeValid_WhenAllRequiredFieldsAreFilled()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Smartphone",
				Description = "<p>Latest model with great features.</p>",
				Image = "http://example.com/image.jpg",
				CategoryId = 1,
				Price = 499.99m,
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void Product_ShouldBeInvalid_WhenNameIsNull()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = null,
				Image = "http://example.com/image.jpg",
				CategoryId = 1,
				Price = 499.99m,
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Name field is required.");
		}

		[Test]
		public void Product_ShouldBeInvalid_WhenNameExceedsMaxLength()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = new string('A', 51), // 51 characters
				Image = "http://example.com/image.jpg",
				CategoryId = 1,
				Price = 499.99m,
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The field Name must be a string or array type with a maximum length of '50'.");
		}

		[Test]
		public void Product_ShouldBeInvalid_WhenPriceIsZeroOrNegative()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Smartphone",
				Image = "http://example.com/image.jpg",
				CategoryId = 1,
				Price = 0m, // Invalid price
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("Price must be a positive value.");
		}

		[Test]
		public void Product_ShouldBeInvalid_WhenAmountIsZeroOrNegative()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Smartphone",
				Image = "http://example.com/image.jpg",
				CategoryId = 1,
				Price = 499.99m,
				Amount = 0 // Invalid amount
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("Amount must be a positive integer.");
		}

		[Test]
		public void Product_ShouldBeValid_WhenImageIsNull()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Smartphone",
				Image = null,
				CategoryId = 1,
				Price = 499.99m,
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void Product_ShouldBeInvalid_WhenImageIsInvalidUrl()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Smartphone",
				Image = "invalid-url",
				CategoryId = 1,
				Price = 499.99m,
				Amount = 10
			};

			// Act
			var validationResults = ValidateProduct(product);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Image field is not a valid fully-qualified http, https, or ftp URL.");
		}

		private IEnumerable<ValidationResult> ValidateProduct(Product product)
		{
			var context = new ValidationContext(product);
			var results = new List<ValidationResult>();

			Validator.TryValidateObject(product, context, results, true);
			return results;
		}
	}
}