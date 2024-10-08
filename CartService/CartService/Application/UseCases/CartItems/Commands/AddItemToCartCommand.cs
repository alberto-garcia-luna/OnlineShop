using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Commands
{
	public record AddItemToCartCommand : IRequest<int>
	{
		public  required CartItem Item { get; set; }
	}

	public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, int>
	{
		private readonly ICartRepository _repository;

		public AddItemToCartCommandHandler(ICartRepository repository)
		{
			_repository = repository;
		}

		public async Task<int> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
		{
			return await _repository.AddItemToCart(request.Item);
		}
	}
}
