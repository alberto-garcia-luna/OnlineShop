using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using LiteDB;

namespace CartService.Infrastructure.Data
{
    public class CartRepository : ICartRepository, IDisposable
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<CartItem> _collection;
		private readonly object _lock = new object();

		public CartRepository(string dbPath)
        {
			_database = new LiteDatabase(dbPath);
            _collection = _database.GetCollection<CartItem>("cartItems");
        }

        public async Task<IEnumerable<CartItem>> GetCartItems(int cartId)
        {
			return await Task.Run(() =>
			{
				lock (_lock)
				{
					return _collection.Find(x => x.CartId == cartId).ToList();
				}
			});
		}

        public async Task<int> AddItemToCart(CartItem item)
        {
			return await Task.Run(() =>
			{
				lock (_lock)
				{
					_collection.Upsert(item);
					return item.Id;
				}
			});
		}

        public async Task RemoveItemFromCart(int cartId, int itemId)
        {
			await Task.Run(() =>
			{
				lock (_lock)
				{
					_collection.DeleteMany(x => x.CartId == cartId && x.Id == itemId);
				}
			});
		}

		public void Dispose()
		{
			_database?.Dispose();
		}
	}
}
