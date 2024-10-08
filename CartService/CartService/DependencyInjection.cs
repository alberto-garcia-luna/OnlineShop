using CartService.Application.Behaviours;
using CartService.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CartService
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddSingleton(new CartRepository("Filename=OnlineShopCartService.db;"));

			services.AddMediatR(cfg => {
				cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
				cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			});

			return services;
		}
	}
}
