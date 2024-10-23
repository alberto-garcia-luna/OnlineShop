using Ardalis.GuardClauses;
using CartService.Application.Interfaces;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Commands
{
	public record RemoveItemFromCartCommand : IRequest
	{
		public required string CartId { get; set; }

		public required int ItemId { get; set; }
	}

	public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand>
	{
		private readonly ICartRepository _repository;

		public RemoveItemFromCartCommandHandler(ICartRepository repository)
        {
			_repository = repository;            
        }

		public async Task Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
		{
			var cartEntity = await _repository.GetCartItems(request.CartId);
			if (cartEntity == null || !cartEntity.Any())
				throw new NotFoundException(request.CartId.ToString(), nameof(cartEntity));

			var cartItemEntity = cartEntity
				.Where(item => item.Id == request.ItemId)
				.FirstOrDefault();
			Guard.Against.NotFound(request.ItemId, cartItemEntity);

			await _repository.RemoveItemFromCart(request.CartId, request.ItemId);
		}
	}
}
