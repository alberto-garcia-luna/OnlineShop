using CatalogService.Application.Common.Models;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using CatalogService.WebApi.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace CatalogService.WebApi.IntegrationTests.Controllers
{
	[TestFixture]
	public class ProductsControllerTests : TestBase
	{
		[Test]
		public async Task GetProduct_InvalidId_ShouldReturnNotFound()
		{
			// Act
			var response = await _client.GetAsync("/api/products/-1");

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}

		[Test]
		public async Task GetProducts_ShouldReturnOk_WithProducts()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var products = new List<Product>
			{
				new Product { Name = "Test Product 1", Category = category, CategoryId = category.Id, Price = 10.0m, Amount = 1 },
				new Product { Name = "Test Product 2", Category = category, CategoryId = category.Id, Price = 15.0m, Amount = 2 }
			};
			_context.Products.AddRange(products);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=10");
			response.EnsureSuccessStatusCode();

			var paginatedProducts = await response.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>();

			// Assert
			paginatedProducts.Should().NotBeNull();
			paginatedProducts.Items.Should().NotBeNull();
			paginatedProducts.Items.Should().NotBeEmpty();
			paginatedProducts.Items.Should().HaveCount(2);
			paginatedProducts.TotalCount.Should().Be(2);
			paginatedProducts.PageNumber.Should().Be(1);
			paginatedProducts.TotalPages.Should().Be(1);
		}

		[Test]
		public async Task GetProducts_WithPagination_ShouldReturnCorrectPage()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			// Add more products than the page size
			var products = new List<Product>
			{
				new Product { Name = "Product 1", Category = category, CategoryId = category.Id, Price = 10.0m, Amount = 1 },
				new Product { Name = "Product 2", Category = category, CategoryId = category.Id, Price = 15.0m, Amount = 2 },
				new Product { Name = "Product 3", Category = category, CategoryId = category.Id, Price = 20.0m, Amount = 3 },
				new Product { Name = "Product 4", Category = category, CategoryId = category.Id, Price = 25.0m, Amount = 4 }
			};
			_context.Products.AddRange(products);
			await _context.SaveChangesAsync();

			// Act - Request first page with page size of 2
			var responsePage1 = await _client.GetAsync("/api/products?pageNumber=1&pageSize=2");
			responsePage1.EnsureSuccessStatusCode();

			var paginatedProductsPage1 = await responsePage1.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>();

			// Assert - Verify first page results
			paginatedProductsPage1.Should().NotBeNull();
			paginatedProductsPage1.Items.Should().HaveCount(2); 
			paginatedProductsPage1.TotalCount.Should().Be(4); 
			paginatedProductsPage1.PageNumber.Should().Be(1);
			paginatedProductsPage1.TotalPages.Should().Be(2); 

			// Act - Request second page
			var responsePage2 = await _client.GetAsync("/api/products?pageNumber=2&pageSize=2");
			responsePage2.EnsureSuccessStatusCode();

			var paginatedProductsPage2 = await responsePage2.Content.ReadFromJsonAsync<PaginatedList<ProductDto>>();

			// Assert - Verify second page results
			paginatedProductsPage2.Should().NotBeNull();
			paginatedProductsPage2.Items.Should().HaveCount(2);
			paginatedProductsPage2.TotalCount.Should().Be(4); 
			paginatedProductsPage2.PageNumber.Should().Be(2);
			paginatedProductsPage2.TotalPages.Should().Be(2); 
		}

		[Test]
		public async Task GetProduct_ValidId_ShouldReturnOk_WithProduct()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var product = new Product { Name = "Test Product", CategoryId = category.Id, Price = 10.0m, Amount = 1 };
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.GetAsync($"/api/products/{product.Id}");
			response.EnsureSuccessStatusCode();

			var returnedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

			// Assert
			returnedProduct.Should().NotBeNull();
			returnedProduct.Name.Should().Be("Test Product");
		}

		[Test]
		public async Task CreateProduct_ShouldReturnCreated()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };
			var newProduct = new ProductDto { Name = "New Product", Category = categoryDto, CategoryId = categoryDto.Id, Price = 15.0m, Amount = 2 };

			// Act
			var response = await _client.PostAsJsonAsync("/api/products", newProduct);
			response.EnsureSuccessStatusCode();

			var createdProductId = await response.Content.ReadAsStringAsync();
			var createdProduct = await _context.Products.FindAsync(int.Parse(createdProductId));

			// Assert
			createdProduct.Should().NotBeNull();
			createdProduct.Name.Should().Be("New Product");
		}

		[Test]
		public async Task UpdateProduct_ValidId_ShouldReturnNoContent()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var product = new Product { Name = "Old Product", Category = category, CategoryId = category.Id, Price = 10.0m, Amount = 1 };
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };
			var updatedProduct = new ProductDto { Id = product.Id, Name = "Updated Product", Category = categoryDto, CategoryId = category.Id, Price = 12.0m, Amount = 2 };

			// Act
			var response = await _client.PutAsJsonAsync($"/api/products/{product.Id}", updatedProduct);
			response.EnsureSuccessStatusCode();

			// Reload context to verify update
			ReloadContext();

			// Assert
			var updatedEntity = await _context.Products.FindAsync(product.Id);
			updatedEntity.Should().NotBeNull();
			updatedEntity.Name.Should().Be("Updated Product");
		}

		[Test]
		public async Task UpdateProduct_InvalidId_ShouldReturnNotFound()
		{
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };

			// Arrange
			var updateProductDto = new ProductDto
			{
				Id = -1,
				Name = "Nonexistent Product",
				Category = categoryDto,
				CategoryId = categoryDto.Id,
				Price = 10.0m,
				Amount = 1
			};

			// Act
			var response = await _client.PutAsJsonAsync("/api/products/-1", updateProductDto);

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}

		[Test]
		public async Task DeleteProduct_ValidId_ShouldReturnNoContent()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var product = new Product { Name = "Product to Delete", Category = category, CategoryId = category.Id, Price = 10.0m, Amount = 1 };
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.DeleteAsync($"/api/products/{product.Id}");
			response.EnsureSuccessStatusCode();

			// Reload context to verify deletion
			ReloadContext();

			// Assert
			var deletedEntity = await _context.Products.FindAsync(product.Id);
			deletedEntity.Should().BeNull();
		}

		[Test]
		public async Task DeleteProduct_InvalidId_ShouldReturnNotFound()
		{
			// Act
			var response = await _client.DeleteAsync("/api/products/-1");

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}
	}
}