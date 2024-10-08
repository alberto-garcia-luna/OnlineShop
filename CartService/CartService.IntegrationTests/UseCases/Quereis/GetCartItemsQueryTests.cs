using CartService.Application.UseCases.CartItems.Queries;
using CartService.Domain.Entities;
using CartService.IntegrationTests.Common;
using FluentAssertions;

namespace CartService.IntegrationTests.UseCases.Quereis
{
    [TestFixture]
    public class GetCartItemsQueryTests : TestBase
    {
        private IEnumerable<CartItem> _cartItems;

        protected async override Task SeedDatabase()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem { Id = 1, CartId = ValidCartId, Name = "Item1", Price = 10.00m, Quantity = 2 },
                new CartItem { Id = 2, CartId = ValidCartId, Name = "Item2", Price = 20.00m, Quantity = 1 }
            };

            foreach (var item in _cartItems)
            {
                await _cartRepository.AddItemToCart(item);
            }
        }

        [Test]
        public async Task Handle_ShouldReturnCartItems_WhenCartIdIsValid()
        {
            // Arrange
            var query = new GetCartItemsQuery { CartId = ValidCartId };

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.Should().BeEquivalentTo(_cartItems);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenCartHasNoItems()
        {
            // Arrange
            var cartId = 2;
            var query = new GetCartItemsQuery { CartId = cartId };

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.Should().BeEmpty();
        }
    }
}