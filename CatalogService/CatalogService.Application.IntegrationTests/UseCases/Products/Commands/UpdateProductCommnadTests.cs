using Ardalis.GuardClauses;
using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Application.UseCases.Products.Commands;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using FluentAssertions;
using FluentValidation;

namespace CatalogService.Application.IntegrationTests.UseCases.Products.Commands
{
	[TestFixture]
	public class UpdateProductCommnadTests : TestBase
	{
		protected async override Task SeedDatabase()
		{
			var category1 = new Category { Id = 1, Name = "Category 1" };
			var category2 = new Category { Id = 2, Name = "Category 2" };

			_context.Categories.AddRange(new List<Category>
			{
				category1,
				category2,
				new Category { Id = 3, Name = "Category 3" }
			});

			await _context.SaveChangesAsync();

			// Seed the in-memory database with test data
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.00m, Amount = 5, CategoryId = category1.Id, Category = category1 },
				new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.00m, Amount = 10, CategoryId = category1.Id, Category = category1 },
				new Product { Id = 3, Name = "Product 3", Description = "Description 3", Price = 30.00m, Amount = 15, CategoryId = category2.Id, Category = category2 },
			};

			await _context.Products.AddRangeAsync(products);
			await _context.SaveChangesAsync();
		}

		[Test]
		public async Task Handle_ShouldUpdateProduct_WhenProductExists()
		{
			// Arrange
			var existingProduct = _context.Products.First();
			var productDto = new ProductDto
			{
				Id = existingProduct.Id,
				Name = "Updated Product",
				Description = "Updated Description",
				Image = "http://example.com/new-image.jpg",
				Amount = 10,
				Price = 59.99m,
				CategoryId = existingProduct.CategoryId,
				Category = new CategoryDto
				{
					Id = existingProduct.CategoryId,
					Name = existingProduct.Category.Name
				}
			};

			var command = new UpdateProductCommnad { Product = productDto };

			// Act
			await _mediator.Send(command);

			// Assert
			var updatedProduct = await _context.Products.FindAsync(existingProduct.Id);
			updatedProduct.Should().NotBeNull();
			updatedProduct.Name.Should().Be(productDto.Name);
			updatedProduct.Description.Should().Be(productDto.Description);
			updatedProduct.Image.Should().Be(productDto.Image);
			updatedProduct.Amount.Should().Be(productDto.Amount);
			updatedProduct.Price.Should().Be(productDto.Price);
			updatedProduct.CategoryId.Should().Be(productDto.CategoryId.Value);
		}

		[Test]
		public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
		{
			// Arrange
			var productDto = new ProductDto
			{
				Id = -1, // Non-existing ID
				Name = "Some Product",
				Description = "Some Description",
				Image = "http://example.com/image.jpg",
				Amount = 5,
				Price = 99.99m,
				CategoryId = -1,
				Category = new CategoryDto
				{
					Id = -1,
					Name = "Product Test"
				}
			};

			var command = new UpdateProductCommnad { Product = productDto };

			// Act
			Func<Task> action = async () => await _mediator.Send(command);

			// Assert
			await action.Should().ThrowAsync<NotFoundException>();
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenProductIsInvalid()
		{
			// Arrange
			var existingProduct = _context.Products.First();
			var productDto = new ProductDto
			{
				Id = existingProduct.Id,
				Name = string.Empty, // Invalid: empty name
				Description = "Updated Description",
				Image = "http://example.com/new-image.jpg",
				Amount = -5, // Invalid: negative amount
				Price = -99.99m, // Invalid: negative price
				CategoryId = existingProduct.CategoryId
			};

			var command = new UpdateProductCommnad { Product = productDto };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Product.Name: 'Product Name' must not be empty. Severity: Error\r\n -- Product.Category: Category must not be null. Severity: Error\r\n -- Product.Price: Price must not be a positive number. Severity: Error\r\n -- Product.Amount: Amount must not be a positive number. Severity: Error");
		}
	}
}