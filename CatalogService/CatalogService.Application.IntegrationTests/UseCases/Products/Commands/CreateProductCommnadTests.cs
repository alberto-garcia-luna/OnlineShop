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
	public class CreateProductCommnadTests : TestBase
	{
		protected async override Task SeedDatabase()
		{
			var categoryId1 = Guid.NewGuid();
			var categoryId2 = Guid.NewGuid();

			_context.Categories.AddRange(new List<Category>
			{
				new Category { Id = categoryId1, Name = "Category 1" },
				new Category { Id = categoryId2, Name = "Category 2" },
				new Category { Id = Guid.NewGuid(), Name = "Category 3" }
			});

			await _context.SaveChangesAsync();
		}

		[Test]
		public async Task Handle_ShouldCreateProduct_WhenProductIsValid()
		{
			// Arrange
			var category = _context.Categories.First();
			var productDto = new ProductDto
			{
				Name = "Test Product",
				Description = "Test Description",
				Image = "http://example.com/image.jpg",
				Amount = 10,
				Price = 99.99m,
				CategoryId = category.Id,
				Category = new CategoryDto { 
					Id = category.Id,
					Name = category.Name
				}
			};

			var command = new CreateProductCommnad { Product = productDto };

			// Act
			var result = await _mediator.Send(command);
			var createdProduct = await _context.Products.FindAsync(result);

			// Assert
			result.Should().NotBe(Guid.Empty); // Ensure a new GUID is returned			
			createdProduct.Should().NotBeNull();
			createdProduct.Name.Should().Be(productDto.Name);
			createdProduct.Description.Should().Be(productDto.Description);
			createdProduct.Image.Should().Be(productDto.Image);
			createdProduct.Amount.Should().Be(productDto.Amount);
			createdProduct.Price.Should().Be(productDto.Price);
			createdProduct.CategoryId.Should().Be(productDto.CategoryId.Value);
		}

		[Test]
		public async Task Handle_ShouldThrowArgumentNullException_WhenProductIsNullAsync()
		{
			// Arrange
			var command = new CreateProductCommnad { Product = null };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Product: Product must not be null. Severity: Error");
		}

		[Test]
		public async Task Handle_ShouldThrowValidationException_WhenProductIsInvalid()
		{
			// Arrange
			var productDto = new ProductDto
			{
				Name = string.Empty, // Invalid: empty name
				Description = "Test Description",
				Image = "invalid-url", // Invalid URL
				Amount = -5, // Invalid: negative amount
				Price = -99.99m, // Invalid: negative price
				CategoryId = null // Missing category
			};

			var command = new CreateProductCommnad { Product = productDto };

			// Act
			Func<Task> act = async () => await _mediator.Send(command);

			// Assert
			await act.Should().ThrowAsync<ValidationException>()
				.WithMessage("Validation failed: \r\n -- Product.Name: 'Product Name' must not be empty. Severity: Error\r\n -- Product.Category: Category must not be null. Severity: Error\r\n -- Product.Price: Price must not be a positive number. Severity: Error\r\n -- Product.Amount: Amount must not be a positive number. Severity: Error");
		}
	}
}
