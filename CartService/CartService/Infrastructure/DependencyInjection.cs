using CartService.Application.Interfaces;
using CartService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Ardalis.GuardClauses;

namespace CartService.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

			services.AddSingleton<ICartRepository>(new CartRepository(connectionString));

			services.AddSingleton(TimeProvider.System);

			return services;
		}
	}
}
