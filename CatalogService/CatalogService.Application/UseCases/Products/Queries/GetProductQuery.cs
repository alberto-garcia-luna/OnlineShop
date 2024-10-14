using AutoMapper;
using AutoMapper.QueryableExtensions;
using CatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.UseCases.Products.Queries
{
	public record GetProductQuery(int Id) : IRequest<ProductDto>;

	public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
	{
		private readonly IApplicationDbContext _context;
		private readonly IMapper _mapper;

		public GetProductQueryHandler(IApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
		{
			return await _context.Products
				.Where(item => item.Id == request.Id)
				.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
				.FirstOrDefaultAsync();
		}
	}
}
