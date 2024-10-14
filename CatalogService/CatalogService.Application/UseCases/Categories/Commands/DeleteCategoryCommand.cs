using Ardalis.GuardClauses;
using CatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.UseCases.Categories.Commands
{
	public record DeleteCategoryCommand(int Id) : IRequest;

	public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
	{
		private readonly IApplicationDbContext _context;

        public DeleteCategoryCommandHandler(IApplicationDbContext context)
		{
            _context = context;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			var entity = await _context.Categories
				.Where(l => l.Id == request.Id)
				.SingleOrDefaultAsync(cancellationToken);

			Guard.Against.NotFound(request.Id, entity);

			_context.Categories.Remove(entity);

			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
