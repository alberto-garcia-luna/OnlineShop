using AutoMapper;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.UseCases.Categories.Queries
{
	public class CategoryDto
	{
		public Guid? Id { get; init; }

		public string? Name { get; init; }

		public string? Image { get; init; }

		public Guid? ParentCategoryId { get; init; }

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
