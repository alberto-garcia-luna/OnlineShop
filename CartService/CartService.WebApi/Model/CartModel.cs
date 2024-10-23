using CartService.Application.UseCases.CartItems.Queries;

namespace CartService.WebApi.Model
{
    public class CartModel
    {
        public string CartId { get; init; }

        public IEnumerable<CartItemDto> CartItems { get; init; }
    }
}
