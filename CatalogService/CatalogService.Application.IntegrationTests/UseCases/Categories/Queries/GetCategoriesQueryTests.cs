using CatalogService.Application.IntegrationTests.Common;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using FluentAssertions;

namespace CatalogService.Application.IntegrationTests.UseCases.Categories.Queries
{
	[TestFixture]
	public class GetCategoriesQueryTests : TestBase
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
        public async Task Handle_ReturnsPaginatedListOfCategories()
        {
			// Arrange
			var query = new GetCategoriesQuery
			{
				PageNumber = 1,
				PageSize = 10
			};

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Items.Should().NotBeNull();
            result.Items.Should().NotBeEmpty();
			result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);
            result.PageNumber.Should().Be(1);
			result.TotalPages.Should().Be(1);
		}

		[Test]
		public async Task Handle_ShouldReturnSecondPageOfProducts_WhenMoreThanPageSizeExist()
		{
			// Arrange
			var query = new GetCategoriesQuery { PageNumber = 2, PageSize = 2 };

			// Act
			var result = await _mediator.Send(query);

			// Assert
			result.Items.Should().NotBeNull();
			result.Items.Should().NotBeEmpty();
			result.Items.Should().HaveCount(1);
			result.TotalCount.Should().Be(3);
			result.PageNumber.Should().Be(2);
			result.TotalPages.Should().Be(2);
		}
	}
}