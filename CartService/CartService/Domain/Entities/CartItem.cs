using System.ComponentModel.DataAnnotations;

namespace CartService.Domain.Entities
{
	public class CartItem
	{
		[Required]
		public required string CartId { get; set; }

		public int Id { get; set; }

		[Required]
		public required string Name { get; set; }

		[Url]
		public string? Image { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		public required decimal Price { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
		public required int Quantity { get; set; }
	}
}
