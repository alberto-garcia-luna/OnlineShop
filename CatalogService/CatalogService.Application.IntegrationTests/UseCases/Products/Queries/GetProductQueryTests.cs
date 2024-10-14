using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using FluentAssertions;

namespace CatalogService.Application.IntegrationTests.UseCases.Products.Queries
{
	[TestFixture]
	public class GetProductQueryTests : TestBase
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

			// Seed the in-memory database with test data
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.00m, Amount = 5, CategoryId = categoryId1 },
				new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.00m, Amount = 10, CategoryId = categoryId1 },
				new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.00m, Amount = 15, CategoryId = categoryId2 },
			};

			await _context.Products.AddRangeAsync(products);
			await _context.SaveChangesAsync();
		}

		[Test]
		public async Task Handle_ShouldReturnProductDto_WhenProductExists()
		{
			// Arrange
			var product = _context.Products.First();
			var query = new GetProductQuery(product.Id);

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Should().NotBeNull();
			result.Name.Should().Be(product.Name);
			result.Description.Should().Be(product.Description);
			result.Image.Should().Be(product.Image);
			result.CategoryId.Should().Be(product.CategoryId);
			result.Price.Should().Be(product.Price);
			result.Amount.Should().Be(product.Amount);
		}

		[Test]
		public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
		{
			// Arrange
			var query = new GetProductQuery(-1);

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Should().BeNull();
		}
	}
}
