using Ardalis.GuardClauses;
using CatalogService.Application.UseCases.Categories.Commands;
using CatalogService.Application.UseCases.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
			_mediator = mediator;
		}

		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
		{
			var query = new GetCategoriesQuery();
			var result = await _mediator.Send(query);

			return Ok(result);
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<CategoryDto>> GetCategory(int id)
		{
			var query = new GetCategoryQuery(id);
			var result = await _mediator.Send(query);

			if (result == null)
				return NotFound();

			result.Links.Add(
				"self", Url.Link(nameof(GetCategory), new { id = result.Id }));
			result.Links.Add(
				"update", Url.Link(nameof(UpdateCategory), new { id = result.Id }));
			result.Links.Add(
				"delete", Url.Link(nameof(DeleteCategory), new { id = result.Id }));

			return Ok(result);
		}

		[HttpPost]
		[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<int>> CreateCategory([FromBody] CategoryDto categoryDto)
		{
			var command = new CreateCategoryCommnad { Category = categoryDto };
			var categoryId = await _mediator.Send(command);

			return CreatedAtAction(nameof(GetCategory), new { id = categoryId }, categoryId);
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
		{
			if (id != categoryDto.Id)
				return BadRequest("Category ID mismatch.");

			try
			{
				var command = new UpdateCategoryCommnad { Category = categoryDto };
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
		public async Task<IActionResult> DeleteCategory(int id)
		{
			try
			{
				var command = new DeleteCategoryCommand(id);
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
