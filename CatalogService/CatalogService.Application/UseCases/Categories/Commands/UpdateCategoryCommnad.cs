using Ardalis.GuardClauses;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Categories.Queries;
using MediatR;

namespace CatalogService.Application.UseCases.Categories.Commands
{
	public record UpdateCategoryCommnad : IRequest
	{
		public required CategoryDto Category { get; set; }
	}

	public class UpdateCategoryCommnadHandler : IRequestHandler<UpdateCategoryCommnad>
	{
		private readonly IApplicationDbContext _context;

        public UpdateCategoryCommnadHandler(IApplicationDbContext context)
		{
            _context = context;
        }

        public async Task Handle(UpdateCategoryCommnad request, CancellationToken cancellationToken)
		{
			var entity = await _context.Categories
				.FindAsync([request.Category.Id], cancellationToken);

			Guard.Against.NotFound(request.Category.Id.Value, entity);

			entity.Name = request.Category.Name;			
			entity.Image = request.Category.Image;
			entity.ParentCategoryId = request.Category.ParentCategoryId;

			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
