using CartService.Domain.Entities;

namespace CartService.Application.Interfaces
{
	public interface ICartRepository
	{
		Task<IEnumerable<CartItem>> GetCartItems(int cartId);

		Task<int> AddItemToCart(CartItem item);

		Task RemoveItemFromCart(int cartId, int itemId);

		Task CleanDatabase();
	}
}
