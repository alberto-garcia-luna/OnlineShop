using Ardalis.GuardClauses;
using Asp.Versioning;
using CartService.Application.UseCases.CartItems.Commands;
using CartService.Application.UseCases.CartItems.Queries;
using CartService.WebApi.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartService.WebApi.Controllers.v2
{
    [ApiController]
	[ApiVersion("2.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class CartController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CartController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{cartId}")]
		[ProducesResponseType(typeof(IEnumerable<CartItemDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> GetCart(string cartId)
		{
			var items = await _mediator.Send(new GetCartItemsQuery { CartId = cartId });

			if (items == null || !items.Any())
				return NotFound();

			return Ok(items);
		}

		[HttpGet("{cartId}/items/{itemId:int}")]
		[ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> GetCartItem(string cartId, int itemId)
		{
			try
			{
				var query = new GetCartItemQuery { CartId = cartId, CartItemId = itemId };
				var cartItem = await _mediator.Send(query);

				return Ok(cartItem);
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost("{cartId}/items")]
		[ProducesResponseType(typeof(AddCartItemModel), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> AddItem(string cartId, [FromBody] CartItemDto item)
		{
			var command = new AddItemToCartCommand { Item = item };
			var cartItemId = await _mediator.Send(command);

			var response = new AddCartItemModel
			{
				CartId = cartId,
				CartItemId = cartItemId
			};

			return CreatedAtAction(nameof(GetCartItem), new { cartId, itemId = cartItemId }, response);
		}

		[HttpDelete("{cartId}/items/{itemId:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> RemoveItem(string cartId, int itemId)
		{
			try
			{
				var command = new RemoveItemFromCartCommand { CartId = cartId, ItemId = itemId };
				await _mediator.Send(command);

				return NoContent();
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}
	}
}