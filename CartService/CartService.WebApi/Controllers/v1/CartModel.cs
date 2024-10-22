using CartService.Application.UseCases.CartItems.Queries;

namespace CartService.WebApi.Controllers.v1
{
	public class CartModel
	{
		public int CartId { get; init; }

		public IEnumerable<CartItemDto> CartItems { get; init; }
	}
}
