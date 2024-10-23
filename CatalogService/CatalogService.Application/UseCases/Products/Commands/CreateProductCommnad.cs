using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using MediatR;

namespace CatalogService.Application.UseCases.Products.Commands
{
	public record CreateProductCommnad : IRequest<int>
	{
		public required ProductDto Product { get; set; }
	}

	public class CreateProductCommnadHandler : IRequestHandler<CreateProductCommnad, int>
	{
		private readonly IApplicationDbContext _context;

		public CreateProductCommnadHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<int> Handle(CreateProductCommnad request, CancellationToken cancellationToken)
		{
			var entity = new Product()
			{
				Name = request.Product.Name,
				Image = request.Product.Image,
				Amount = request.Product.Amount,
				Price = request.Product.Price,
				Description = request.Product.Description,
				CategoryId = request.Product.CategoryId.Value
			};

			_context.Products.Add(entity);
			await _context.SaveChangesAsync(cancellationToken);

			return entity.Id;
		}
	}
}
