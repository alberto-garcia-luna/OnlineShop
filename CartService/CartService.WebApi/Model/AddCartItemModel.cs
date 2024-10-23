namespace CartService.WebApi.Model
{
    public class AddCartItemModel
    {
        public required string CartId { get; set; }
        public required int CartItemId { get; set; }
    }
}
