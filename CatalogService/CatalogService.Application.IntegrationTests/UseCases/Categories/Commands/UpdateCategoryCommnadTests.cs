using Ardalis.GuardClauses;
using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Commands;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Domain.Entities;
using FluentAssertions;
using FluentValidation;

namespace CatalogService.Application.IntegrationTests.UseCases.Categories.Commands
{
	[TestFixture]
	public class UpdateCategoryCommnadTests : TestBase
	{
		protected async override Task SeedDatabase()
		{
			// Seed the in-memory database with test data
			_context.Categories.AddRange(new List<Category>
			{
				new Category { Id = 1, Name = "Category 1" },
				new Category { Id = 2, Name = "Category 2" },
				new Category { Id = 3, Name = "Category 3" }
			});

			await _context.SaveChangesAsync();
		}

		[Test]
		public async Task Handle_UpdatesCategorySuccessfully()
		{
			// Arrange
			var categoryId = _context.Categories.First().Id;
			var updateCommand = new UpdateCategoryCommnad
			{
				Category = new CategoryDto
				{
					Id = categoryId,
					Name = "Updated Category",
					Image = "updated_image_url",
					ParentCategoryId = null
				}
			};

			// Act
			await _mediator.Send(updateCommand);

			// Assert
			var updatedCategory = await _context.Categories.FindAsync(categoryId);
			updatedCategory.Should().NotBeNull();
			updatedCategory.Name.Should().Be("Updated Category");
			updatedCategory.Image.Should().Be("updated_image_url");
		}

		[Test]
		public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
		{
			// Arrange
			var nonExistentId = -1; // A non-existent category ID
			var updateCommand = new UpdateCategoryCommnad
			{
				Category = new CategoryDto
				{
					Id = nonExistentId,
					Name = "Non-existent Category",
					Image = "non_existent_image_url",
					ParentCategoryId = null
				}
			};

			// Act
			Func<Task> action = async () => await _mediator.Send(updateCommand);

			// Assert
			await action.Should().ThrowAsync<NotFoundException>();
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenCategoryIsInvalid()
		{
			// Arrange
			var categoryId = _context.Categories.First().Id;
			var command = new UpdateCategoryCommnad
			{
				Category = new CategoryDto
				{
					Id = categoryId,
					Name = string.Empty,
					Image = "updated_image_url",
					ParentCategoryId = null
				}
			};

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Category.Name: 'Category Name' must not be empty. Severity: Error");
		}
	}
}