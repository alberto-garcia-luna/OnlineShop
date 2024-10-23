using CartService.Application.UseCases.CartItems.Queries;
using CartService.Domain.Entities;
using CartService.UnitTests.Application.Common;
using FluentAssertions;
using Moq;

namespace CartService.UnitTests.Application.UseCases.Queries
{
    [TestFixture]
    public class GetCartItemsQueryTests : TestBase
    {
        private IEnumerable<CartItem> _cartItems;

        protected override void SeedDatabase()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem { Id = 1, CartId = ValidCartId, Name = "Item1", Price = 10.00m, Quantity = 2 },
                new CartItem { Id = 2, CartId = ValidCartId, Name = "Item2", Price = 20.00m, Quantity = 1 }
            };

            _mockRepository.Setup(repo => repo.GetCartItems(ValidCartId))
                .ReturnsAsync(_cartItems);
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
            _mockRepository.Verify(repo => repo.GetCartItems(ValidCartId), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenCartHasNoItems()
        {
            // Arrange
            string cartId = "CartUniqueKey-123";

            _mockRepository.Setup(repo => repo.GetCartItems(cartId))
                .ReturnsAsync(new List<CartItem>());

            var query = new GetCartItemsQuery { CartId = cartId };

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.Should().BeEmpty();
            _mockRepository.Verify(repo => repo.GetCartItems(cartId), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldCallRepositoryWithCorrectCartId()
        {
            // Arrange
            var query = new GetCartItemsQuery { CartId = ValidCartId };

            // Act
            await _mediator.Send(query);

            // Assert
            _mockRepository.Verify(repo => repo.GetCartItems(ValidCartId), Times.Once);
        }
    }
}