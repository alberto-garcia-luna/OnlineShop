using AutoMapper;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.UseCases.Products.Queries
{
	public class ProductDto
    {
        public int Id { get; init; }

        public string? Name { get; init; }

        public string? Description { get; init; }

        public string? Image { get; init; }

        public int? CategoryId { get; init; }

        public CategoryDto? Category { get; init; }

        public decimal Price { get; init; }

        public int Amount { get; init; }

		private class Mapping : Profile
		{
			public Mapping()
			{
				CreateMap<Product, ProductDto>();
			}
		}
	}
}
