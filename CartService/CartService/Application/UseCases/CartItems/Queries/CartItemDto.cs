using AutoMapper;
using CartService.Domain.Entities;

namespace CartService.Application.UseCases.CartItems.Queries
{
	public class CartItemDto
	{
		public int CartId { get; init; }

		public int Id { get; init; }

		public string? Name { get; init; }

		public string? Image { get; init; }

		public decimal? Price { get; init; }

		public int? Quantity { get; init; }

		private class Mapping : Profile
		{
			public Mapping()
			{
				CreateMap<CartItem, CartItemDto>();
			}
		}
	}
}
