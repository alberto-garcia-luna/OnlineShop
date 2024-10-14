using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public required string Name { get; set; }

		[Url]
		public string? Image { get; set; }

		public int? ParentCategoryId { get; set; }

		public virtual Category? ParentCategory { get; set; }
	}
}
