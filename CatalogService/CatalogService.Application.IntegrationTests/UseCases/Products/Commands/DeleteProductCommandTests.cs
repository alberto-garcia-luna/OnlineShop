using Ardalis.GuardClauses;
using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Products.Commands;
using CatalogService.Domain.Entities;
using FluentAssertions;

namespace CatalogService.Application.IntegrationTests.UseCases.Products.Commands
{
	[TestFixture]
	public class DeleteProductCommandTests : TestBase
	{
		protected async override Task SeedDatabase()
		{
			var categoryId1 = 1;
			var categoryId2 = 2;

			_context.Categories.AddRange(new List<Category>
			{
				new Category { Id = categoryId1, Name = "Category 1" },
				new Category { Id = categoryId2, Name = "Category 2" },
				new Category { Id = 3, Name = "Category 3" }
			});

			await _context.SaveChangesAsync();
		}

		[Test]
		public async Task Handle_ShouldDeleteProduct_WhenProductExists()
		{
			// Arrange
			var product = new Product
			{
				Id = 1,
				Name = "Test Product",
				Description = "Test Description",
				Image = "http://example.com/image.jpg",
				Amount = 10,
				Price = 99.99m,
				CategoryId = _context.Categories.First().Id
			};
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();

			var command = new DeleteProductCommand(product.Id);

			// Act
			await _mediator.Send(command);

			// Assert
			var deletedProduct = await _context.Products.FindAsync(product.Id);
			deletedProduct.Should().BeNull();
		}

		[Test]
		public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
		{
			// Arrange
			var command = new DeleteProductCommand(-1); // Non-existing ID

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<NotFoundException>();
		}
	}
}
