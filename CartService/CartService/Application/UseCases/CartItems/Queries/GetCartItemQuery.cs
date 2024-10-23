using Ardalis.GuardClauses;
using AutoMapper;
using CartService.Application.Interfaces;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Queries
{
	public record GetCartItemQuery : IRequest<CartItemDto>
	{
		public required string CartId { get; set; }

		public required int CartItemId { get; set; }
	}

	public class GetCartItemQueryHandler : IRequestHandler<GetCartItemQuery, CartItemDto>
	{
		private readonly ICartRepository _repository;
		private readonly IMapper _mapper;

		public GetCartItemQueryHandler(ICartRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<CartItemDto> Handle(GetCartItemQuery request, CancellationToken cancellationToken)
		{
			var entity = await _repository.GetCartItem(request.CartId, request.CartItemId);
			Guard.Against.NotFound(request.CartItemId, entity);

			return _mapper.Map<CartItemDto>(entity);
		}
	}
}
