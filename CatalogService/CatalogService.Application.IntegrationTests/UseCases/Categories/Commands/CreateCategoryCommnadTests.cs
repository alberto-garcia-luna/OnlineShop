using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Commands;
using CatalogService.Application.UseCases.Categories.Queries;
using FluentAssertions;
using FluentValidation;

namespace CatalogService.Application.IntegrationTests.UseCases.Categories.Commands
{
	[TestFixture]
	public class CreateCategoryCommnadTests : TestBase
	{
		[Test]
		public async Task Handle_CreatesCategorySuccessfully()
		{
			// Arrange
			var command = new CreateCategoryCommnad
			{
				Category = new CategoryDto
				{
					Name = "New Category",
					Image = "image_url",
					ParentCategoryId = null
				}
			};

			// Act
			var categoryId = await _mediator.Send(command);

			// Assert
			var createdCategory = await _context.Categories.FindAsync(categoryId);
			createdCategory.Should().NotBeNull();
			createdCategory.Name.Should().Be("New Category");
			createdCategory.Image.Should().Be("image_url");
		}

		[Test]
		public async Task Handle_ShouldThrowException_WhenCategoryIsNull()
		{
			// Arrange
			var command = new CreateCategoryCommnad
			{
				Category = null // This should cause an exception
			};

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Category: Category must not be null. Severity: Error");
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenCategoryIsInvalid()
		{
			// Arrange
			var categoryDto = new CategoryDto
			{
				Name = string.Empty, // Invalid: empty name
				Image = "invalid-url", // Invalid URL
				ParentCategoryId = null // Missing category
			};

			var command = new CreateCategoryCommnad { Category = categoryDto };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Category.Name: 'Category Name' must not be empty. Severity: Error");
		}
	}
}
