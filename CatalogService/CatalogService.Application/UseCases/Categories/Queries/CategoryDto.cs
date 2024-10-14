using AutoMapper;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.UseCases.Categories.Queries
{
	public class CategoryDto
	{
		public int Id { get; init; }

		public string? Name { get; init; }

		public string? Image { get; init; }

		public int? ParentCategoryId { get; init; }

		public virtual CategoryDto? ParentCategory { get; init; }

		private class Mapping : Profile
		{
			public Mapping()
			{
				CreateMap<Category, CategoryDto>();
			}
		}
	}
}
