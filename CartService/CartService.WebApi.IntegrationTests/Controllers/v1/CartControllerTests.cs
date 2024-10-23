using CartService.Application.UseCases.CartItems.Queries;
using CartService.WebApi.IntegrationTests.Common;
using CartService.WebApi.Model;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CartService.WebApi.IntegrationTests.Controllers.v1
{
    [TestFixture]
	public class CartControllerTests : TestBase
	{
		[Test]
		public async Task GetCart_ReturnsOk_WhenCartExists()
		{
			// Arrange: Add an item to the cart first
			var item = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "Test Item",
				Price = 10.0m,
				Quantity = 1
			};

			await _client.PostAsJsonAsync($"/api/v1.0/cart/{ValidCartId}/items", item);

			// Act
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var result = await response.Content.ReadFromJsonAsync<CartModel>();

			Assert.IsNotNull(result);
			Assert.AreEqual(ValidCartId, result.CartId);
			Assert.IsNotEmpty(result.CartItems);
		}

		[Test]
		public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Act
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task GetCartItem_ReturnsOk_WhenItemExists()
		{
			// Arrange: Add an item to the cart first
			var item = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "Test Item",
				Price = 10.0m,
				Quantity = 1
			};

			var addedResponse = await _client.PostAsJsonAsync($"/api/v1.0/cart/{ValidCartId}/items", item);
			addedResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			var addCartItemModel = await addedResponse.Content.ReadFromJsonAsync<AddCartItemModel>();

			// Act
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}/items/{addCartItemModel.CartItemId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var result = await response.Content.ReadFromJsonAsync<CartItemDto>();
			Assert.IsNotNull(result);
			Assert.AreEqual(item.Name, result.Name);
			Assert.AreEqual(item.Price, result.Price);
			Assert.AreEqual(item.Quantity, result.Quantity);
		}

		[Test]
		public async Task GetCartItem_ReturnsNotFound_WhenItemDoesNotExist()
		{
			// Act
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}/items/-1");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task AddItem_ReturnsCreated_WhenItemIsValid()
		{
			// Arrange
			var item = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "New Item",
				Price = 20.0m,
				Quantity = 1
			};

			// Act
			var addedResponse = await _client.PostAsJsonAsync($"/api/v1.0/cart/{ValidCartId}/items", item);
			addedResponse.StatusCode.Should().Be(HttpStatusCode.Created);
			await addedResponse.Content.ReadFromJsonAsync<AddCartItemModel>();

			// Assert
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Test]
		public async Task RemoveItem_ReturnsNoContent_WhenItemExists()
		{
			// Arrange: Add an item to the cart first
			var item = new CartItemDto
			{
				CartId = ValidCartId,
				Name = "Item to Remove",
				Price = 5.0m,
				Quantity = 1
			};
			var addedResponse = await _client.PostAsJsonAsync($"/api/v1.0/cart/{ValidCartId}/items", item);
			addedResponse.StatusCode.Should().Be(HttpStatusCode.Created);

			var addCartItemModel = await addedResponse.Content.ReadFromJsonAsync<AddCartItemModel>();
			addCartItemModel.Should().NotBeNull();

			// Act
			var deleteResponse = await _client.DeleteAsync($"/api/v1.0/cart/{ValidCartId}/items/{addCartItemModel.CartItemId}");

			// Assert
			deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

			// Check that the item is not found anymore
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}/items/{addCartItemModel.CartItemId}");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task RemoveItem_ReturnsNotFound_WhenItemDoesNotExist()
		{
			// Act
			var response = await _client.DeleteAsync($"/api/v1.0/cart/{ValidCartId}/items/-1");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
