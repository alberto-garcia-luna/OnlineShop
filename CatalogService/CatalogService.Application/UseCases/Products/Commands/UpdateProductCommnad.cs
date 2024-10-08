using Ardalis.GuardClauses;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Products.Queries;
using MediatR;

namespace CatalogService.Application.UseCases.Products.Commands
{
	public record UpdateProductCommnad : IRequest
	{
		public required ProductDto Product { get; set; }
	}

	public class UpdateProductCommnadHandler : IRequestHandler<UpdateProductCommnad>
	{
		private readonly IApplicationDbContext _context;

		public UpdateProductCommnadHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task Handle(UpdateProductCommnad request, CancellationToken cancellationToken)
		{
			var entity = await _context.Products
				.FindAsync([request.Product.Id], cancellationToken);

			Guard.Against.NotFound(request.Product.Id.Value, entity);

			entity.Name = request.Product.Name;
			entity.Image = request.Product.Image;
			entity.Description = request.Product.Description;
			entity.Price = request.Product.Price;
			entity.Amount = request.Product.Amount;
			entity.CategoryId = request.Product.CategoryId.Value;

			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
