using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Domain.Entities;
using CatalogService.WebApi.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace CatalogService.WebApi.IntegrationTests.Controllers
{
	[TestFixture]
	public class CategoriesControllerTests : TestBase
	{
		[Test]
		public async Task GetCategories_ShouldReturnOk_WithCategories()
		{
			// Arrange
			_context.Categories.Add(new Category { Name = "Test Category" });
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.GetAsync("/api/categories");
			response.EnsureSuccessStatusCode();

			var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();

			// Assert
			categories.Should().NotBeNull();
			categories.Should().HaveCount(1);
			categories.Should().ContainSingle(c => c.Name == "Test Category");
		}

		[Test]
		public async Task GetCategory_ValidId_ShouldReturnOk_WithCategory()
		{
			// Arrange
			var category = new Category { Name = "Test Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.GetAsync($"/api/categories/{category.Id}");
			response.EnsureSuccessStatusCode();

			var returnedCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();

			// Assert
			returnedCategory.Should().NotBeNull();
			returnedCategory.Name.Should().Be("Test Category");
		}

		[Test]
		public async Task GetCategory_InvalidId_ShouldReturnNotFound()
		{
			// Act
			var response = await _client.GetAsync("/api/categories/-1");

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}

		[Test]
		public async Task CreateCategory_ShouldReturnCreated()
		{
			// Arrange
			var newCategory = new CategoryDto { Name = "New Category" };

			// Act
			var response = await _client.PostAsJsonAsync("/api/categories", newCategory);
			response.EnsureSuccessStatusCode();

			var createdCategoryId = await response.Content.ReadAsStringAsync();
			var createdCategory = await _context.Categories.FindAsync(int.Parse(createdCategoryId));

			// Assert
			createdCategory.Should().NotBeNull();
			createdCategory.Name.Should().Be("New Category");
		}

		[Test]
		public async Task UpdateCategory_ValidId_ShouldReturnNoContent()
		{
			// Arrange
			var category = new Category { Name = "Old Category" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			var updatedCategory = new CategoryDto { Id = category.Id, Name = "Updated Category" };

			// Act
			var response = await _client.PutAsJsonAsync($"/api/categories/{category.Id}", updatedCategory);
			response.EnsureSuccessStatusCode();

			// Reload context to verify update
			ReloadContext();

			// Assert
			var updatedEntity = await _context.Categories.FindAsync(category.Id);

			updatedEntity.Should().NotBeNull();
			updatedEntity.Name.Should().Be("Updated Category");
		}

		[Test]
		public async Task UpdateCategory_InvalidId_ShouldReturnNotFound()
		{
			// Arrange
			var updatedCategory = new CategoryDto { Id = -1, Name = "Updated Category" };

			// Act
			var response = await _client.PutAsJsonAsync("/api/categories/-1", updatedCategory);

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}

		[Test]
		public async Task DeleteCategory_ValidId_ShouldReturnNoContent()
		{
			// Arrange
			var category = new Category { Name = "Category to Delete" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.DeleteAsync($"/api/categories/{category.Id}");
			response.EnsureSuccessStatusCode();

			// Reload context to verify deletion
			ReloadContext();

			// Assert
			var deletedEntity = await _context.Categories.FindAsync(category.Id);

			deletedEntity.Should().BeNull();
		}

		[Test]
		public async Task DeleteCategory_InvalidId_ShouldReturnNotFound()
		{
			// Act
			var response = await _client.DeleteAsync("/api/categories/-1");

			// Assert
			((int)response.StatusCode).Should().Be(StatusCodes.Status404NotFound);
		}


		[Test]
		public async Task DeleteCategory_ValidId_ShouldDeleteAssociatedProducts()
		{
			// Arrange
			var category = new Category { Name = "Category to Delete" };
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			// Add associated products
			var product1 = new Product { Name = "Product 1", CategoryId = category.Id, Price = 10.0m, Amount = 1 };
			var product2 = new Product { Name = "Product 2", CategoryId = category.Id, Price = 20.0m, Amount = 2 };
			_context.Products.AddRange(product1, product2);
			await _context.SaveChangesAsync();

			// Act
			var response = await _client.DeleteAsync($"/api/categories/{category.Id}");
			response.EnsureSuccessStatusCode();

			// Reload context to verify deletion
			ReloadContext();

			// Assert that the category has been deleted
			var deletedCategory = await _context.Categories.FindAsync(category.Id);
			deletedCategory.Should().BeNull();

			// Assert that associated products have been deleted
			var deletedProducts = await _context.Products.Where(p => p.CategoryId == category.Id).ToListAsync();
			deletedProducts.Should().BeEmpty(); // Verify that the list of products is empty
		}
	}
}