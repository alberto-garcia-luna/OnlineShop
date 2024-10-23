using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Domain.Entities;
using MediatR;

namespace CatalogService.Application.UseCases.Categories.Commands
{
	public record CreateCategoryCommnad : IRequest<int>
	{
        public required CategoryDto Category { get; set; }
    }

	public class CreateCategoryCommnadHandler : IRequestHandler<CreateCategoryCommnad, int>
	{
		private readonly IApplicationDbContext _context;

		public CreateCategoryCommnadHandler(IApplicationDbContext context)
		{
            _context = context;
		}

		public async Task<int> Handle(CreateCategoryCommnad request, CancellationToken cancellationToken)
		{
			var entity = new Category()
			{
				Name = request.Category.Name,
				Image = request.Category.Image,
				ParentCategoryId = request.Category.ParentCategoryId
			};
						
			_context.Categories.Add(entity);
			await _context.SaveChangesAsync(cancellationToken);

			return entity.Id;
		}
	}
}
