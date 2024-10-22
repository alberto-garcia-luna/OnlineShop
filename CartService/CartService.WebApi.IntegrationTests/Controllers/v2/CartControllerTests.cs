using CartService.Application.UseCases.CartItems.Queries;
using CartService.WebApi.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CartService.WebApi.IntegrationTests.Controllers.v2
{
	[TestFixture]
	public class CartControllerTests : TestBase
	{
		[Test]
		[NonParallelizable]
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

			await _client.PostAsJsonAsync($"/api/v2.0/cart/{ValidCartId}/items", item);

			// Act
			var response = await _client.GetAsync($"/api/v2.0/cart/{ValidCartId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var result = await response.Content.ReadFromJsonAsync<IEnumerable<CartItemDto>>();

			result.Should().NotBeNull();
			result.Should().ContainSingle(); // Adjust based on expected items
			result.First().CartId.Should().Be(ValidCartId);
		}

		[Test]
		[NonParallelizable]
		public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
		{
			// Act
			var response = await _client.GetAsync($"/api/v2.0/cart/{ValidCartId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		[NonParallelizable]
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
			var addedResponse = await _client.PostAsJsonAsync($"/api/v2.0/cart/{ValidCartId}/items", item);
			addedResponse.StatusCode.Should().Be(HttpStatusCode.Created);

			var cartItemId = await addedResponse.Content.ReadFromJsonAsync<int>();

			// Assert
			var response = await _client.GetAsync($"/api/v1.0/cart/{ValidCartId}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Test]
		[NonParallelizable]
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
			var addedResponse = await _client.PostAsJsonAsync($"/api/v2.0/cart/{ValidCartId}/items", item);
			var cartItemId = await addedResponse.Content.ReadFromJsonAsync<int>();

			// Act
			var response = await _client.DeleteAsync($"/api/v2.0/cart/{ValidCartId}/items/{cartItemId}");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		[Test]
		[NonParallelizable]
		public async Task RemoveItem_ReturnsNotFound_WhenItemDoesNotExist()
		{
			// Act
			var response = await _client.DeleteAsync($"/api/v2.0/cart/{ValidCartId}/items/-1");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}