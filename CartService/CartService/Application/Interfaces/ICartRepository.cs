using CartService.Domain.Entities;

namespace CartService.Application.Interfaces
{
	public interface ICartRepository
	{
		Task<IEnumerable<CartItem>> GetCartItems(string cartId);

		Task<CartItem> GetCartItem(string cartId, int cartItemId);

		Task<int> AddItemToCart(CartItem item);

		Task RemoveItemFromCart(string cartId, int itemId);

		Task CleanDatabase();
	}
}
