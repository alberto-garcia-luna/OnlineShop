using AutoMapper;
using CartService.Application.Interfaces;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Queries
{
	public record GetCartItemsQuery : IRequest<IEnumerable<CartItemDto>>
	{
		public required string CartId { get; set; }
	}

	public class GetCartItemsQueryHandler : IRequestHandler<GetCartItemsQuery, IEnumerable<CartItemDto>>
	{
		private readonly ICartRepository _repository;
		private readonly IMapper _mapper;

		public GetCartItemsQueryHandler(ICartRepository repository, IMapper mapper)
        {
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<CartItemDto>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
		{
			var cartItems = await _repository.GetCartItems(request.CartId);
			return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
		}
	}
}
