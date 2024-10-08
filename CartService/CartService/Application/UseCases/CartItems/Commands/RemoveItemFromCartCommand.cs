using CartService.Application.Interfaces;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Commands
{
	public record RemoveItemFromCartCommand : IRequest
	{
		public int CartId { get; set; }

		public int ItemId { get; set; }
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
			await _repository.RemoveItemFromCart(request.CartId, request.ItemId);
		}
	}
}
