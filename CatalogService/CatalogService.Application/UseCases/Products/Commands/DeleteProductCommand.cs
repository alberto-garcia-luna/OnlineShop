using Ardalis.GuardClauses;
using CatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.UseCases.Products.Commands
{
	public record DeleteProductCommand(int Id) : IRequest;

	public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
	{
		private readonly IApplicationDbContext _context;

		public DeleteProductCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			var entity = await _context.Products
				.Where(l => l.Id == request.Id)
				.SingleOrDefaultAsync(cancellationToken);

			Guard.Against.NotFound(request.Id, entity);

			_context.Products.Remove(entity);

			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
