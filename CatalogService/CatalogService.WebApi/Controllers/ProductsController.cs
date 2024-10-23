using Ardalis.GuardClauses;
using CatalogService.Application.Common.Models;
using CatalogService.Application.UseCases.Products.Commands;
using CatalogService.Application.UseCases.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ProductsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[ProducesResponseType(typeof(PaginatedList<ProductDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts(
		   [FromQuery] int? categoryId,
		   [FromQuery] int pageNumber = 1,
		   [FromQuery] int pageSize = 10)
		{
			var query = new GetProductsQuery { PageNumber = pageNumber, PageSize = pageSize };
			var products = await _mediator.Send(query);

			return Ok(products);
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ProductDto>> GetProduct(int id)
		{
			var query = new GetProductQuery(id);
			var product = await _mediator.Send(query);

			if (product == null)
				return NotFound();

			product.Links.Add(
				"self", Url.Link(nameof(GetProduct), new { id = product.Id }));
			product.Links.Add(
				"update", Url.Link(nameof(UpdateProduct), new { id = product.Id }));
			product.Links.Add(
				"delete", Url.Link(nameof(DeleteProduct), new { id = product.Id }));

			return Ok(product);
		}

		[HttpPost]
		[ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]		
		public async Task<ActionResult<int>> AddProduct([FromBody] ProductDto productDto)
		{
			var command = new CreateProductCommnad { Product = productDto };
			var productId = await _mediator.Send(command);

			return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]		
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
		{
			if (id != productDto.Id)
				return BadRequest("Product ID mismatch.");

			try
			{
				var command = new UpdateProductCommnad { Product = productDto };
				await _mediator.Send(command);

				return NoContent();
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			try
			{
				var command = new DeleteProductCommand(id);
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
