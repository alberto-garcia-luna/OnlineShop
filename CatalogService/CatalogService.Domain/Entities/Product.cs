using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
	public class Product
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public required string Name { get; set; }

		public string? Description { get; set; }

		[Url]
		public string? Image { get; set; }

		[Required]
		public required int CategoryId { get; set; }

		public virtual Category Category { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		public required decimal Price { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer.")]
		public required int Amount { get; set; }
    }
}
