using CatalogService.Domain.Entities;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.UnitTests
{
	[TestFixture]
	public class CategoryTests
	{
		[Test]
		public void Category_ShouldBeValid_WhenAllRequiredFieldsAreFilled()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Electronics",
				Image = "http://example.com/image.jpg",
				ParentCategoryId = null
			};

			// Act
			var validationResults = ValidateCategory(category);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void Category_ShouldBeInvalid_WhenNameIsNull()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = null,
				Image = "http://example.com/image.jpg",
				ParentCategoryId = null
			};

			// Act
			var validationResults = ValidateCategory(category);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Name field is required.");
		}

		[Test]
		public void Category_ShouldBeInvalid_WhenNameExceedsMaxLength()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = new string('A', 51), // 51 characters
				Image = "http://example.com/image.jpg",
				ParentCategoryId = null
			};

			// Act
			var validationResults = ValidateCategory(category);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The field Name must be a string or array type with a maximum length of '50'.");
		}

		[Test]
		public void Category_ShouldBeValid_WhenImageIsNull()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Books",
				Image = null,
				ParentCategoryId = null
			};

			// Act
			var validationResults = ValidateCategory(category);

			// Assert
			validationResults.Should().BeEmpty();
		}

		[Test]
		public void Category_ShouldBeInvalid_WhenImageIsInvalidUrl()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Books",
				Image = "invalid-url",
				ParentCategoryId = null
			};

			// Act
			var validationResults = ValidateCategory(category);

			// Assert
			validationResults.Should().ContainSingle()
				.Which.ErrorMessage.Should().Be("The Image field is not a valid fully-qualified http, https, or ftp URL.");
		}

		private IEnumerable<ValidationResult> ValidateCategory(Category category)
		{
			var context = new ValidationContext(category);
			var results = new List<ValidationResult>();

			Validator.TryValidateObject(category, context, results, true);
			return results;
		}
	}
}