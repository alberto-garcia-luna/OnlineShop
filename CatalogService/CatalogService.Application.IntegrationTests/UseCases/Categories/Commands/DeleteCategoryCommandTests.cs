using Ardalis.GuardClauses;
using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Commands;
using CatalogService.Domain.Entities;
using FluentAssertions;

namespace CatalogService.Application.IntegrationTests.UseCases.Categories.Commands
{
	[TestFixture]
	public class DeleteCategoryCommandTests : TestBase
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
		public async Task Handle_DeletesCategorySuccessfully()
		{
			// Arrange
			var categoryId = _context.Categories.First().Id;
			var command = new DeleteCategoryCommand(categoryId);

			// Act
			await _mediator.Send(command);

			// Assert
			var deletedCategory = await _context.Categories.FindAsync(categoryId);
			deletedCategory.Should().BeNull();
		}

		[Test]
		public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
		{
			// Arrange
			var nonExistentId = -1; // A non-existent category ID
			var command = new DeleteCategoryCommand(nonExistentId);

			// Act
			Func<Task> action = async () => await _mediator.Send(command);

			// Assert
			await action.Should().ThrowAsync<NotFoundException>();
		}
	}
}