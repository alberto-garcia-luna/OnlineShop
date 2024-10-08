using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Domain.Entities;
using FluentAssertions;

namespace CatalogService.Application.IntegrationTests.UseCases.Categories.Queries
{
	[TestFixture]
	public class GetCategoryQueryTests : TestBase
    {
        protected async override Task SeedDatabase()
        {
            // Seed the in-memory database with test data
            _context.Categories.AddRange(new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                new Category { Id = Guid.NewGuid(), Name = "Category 2" },
                new Category { Id = Guid.NewGuid(), Name = "Category 3" }
            });

            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task Handle_ReturnsCategoryDto_WhenCategoryExists()
        {
            // Arrange
            var categoryId = _context.Categories.First().Id;
            var query = new GetCategoryQuery 
            { 
                Id = categoryId.Value 
            };

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
        }

        [Test]
        public async Task Handle_ReturnsNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var query = new GetCategoryQuery 
            { 
                Id = Guid.NewGuid() // Non-existing ID
			};

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Should().BeNull();
        }
    }
}
