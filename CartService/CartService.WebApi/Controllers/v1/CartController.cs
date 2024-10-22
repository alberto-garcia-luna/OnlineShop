using Ardalis.GuardClauses;
using Asp.Versioning;
using CartService.Application.UseCases.CartItems.Commands;
using CartService.Application.UseCases.CartItems.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartService.WebApi.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{cartId}")]
        [ProducesResponseType(typeof(CartModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCart(int cartId)
        {
            var items = await _mediator.Send(new GetCartItemsQuery { CartId = cartId });

            if (items == null || !items.Any())
                return NotFound();

            return Ok(new CartModel { CartId = cartId, CartItems = items });
        }

        [HttpPost("{cartId:int}/items")]
        [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddItem(int cartId, [FromBody] CartItemDto item)
        {
			var command = new AddItemToCartCommand { Item = item };
			var cartItemId = await _mediator.Send(command);

			return CreatedAtAction(nameof(GetCart), new { cartId }, cartId);
        }

        [HttpDelete("{cartId:int}/items/{itemId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveItem(int cartId, int itemId)
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