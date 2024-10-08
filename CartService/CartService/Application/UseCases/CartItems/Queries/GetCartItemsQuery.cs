using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Queries
{
	public record GetCartItemsQuery : IRequest<IEnumerable<CartItem>>
	{
		public required int CartId { get; set; }
	}

	public class GetCartItemsQueryHandler : IRequestHandler<GetCartItemsQuery, IEnumerable<CartItem>>
	{
		private readonly ICartRepository _repository;

        public GetCartItemsQueryHandler(ICartRepository repository)
        {
			_repository = repository;
        }

		public async Task<IEnumerable<CartItem>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
		{
			return await _repository.GetCartItems(request.CartId);
		}
	}
}
