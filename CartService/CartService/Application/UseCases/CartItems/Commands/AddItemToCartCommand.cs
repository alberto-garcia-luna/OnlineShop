using CartService.Application.Interfaces;
using CartService.Application.UseCases.CartItems.Queries;
using CartService.Domain.Entities;
using MediatR;

namespace CartService.Application.UseCases.CartItems.Commands
{
	public record AddItemToCartCommand : IRequest<int>
	{
		public required CartItemDto Item { get; set; }
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
			var entity = new CartItem
			{ 
				CartId = request.Item.CartId,
				Name = request.Item.Name,
				Image = request.Item.Image,
				Price = request.Item.Price.Value,
				Quantity = request.Item.Quantity.Value
			};

			return await _repository.AddItemToCart(entity);
		}
	}
}
