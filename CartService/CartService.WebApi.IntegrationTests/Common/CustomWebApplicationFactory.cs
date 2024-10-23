using Ardalis.GuardClauses;
using CartService.Application.Interfaces;
using CartService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.WebApi.IntegrationTests.Common
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{

			builder.ConfigureAppConfiguration((context, config) =>
			{
				var basePath = Path.GetDirectoryName(typeof(TStartup).Assembly.Location);
				Guard.Against.Null(basePath, "Project root could not be located.");

				// Clear existing configuration
				config.Sources.Clear();

				// Set up configuration sources
				config
					.SetBasePath(basePath)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables();
			});

			builder.ConfigureTestServices(services =>
			{
				var serviceProvider = services.BuildServiceProvider();
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();
				var connectionString = configuration.GetConnectionString("DefaultConnection");

				Guard.Against.Null(connectionString, "Connection string 'DefaultConnection' not found.");

				var repository = serviceProvider.GetRequiredService<ICartRepository>();
				if (repository == null)
					services.AddSingleton<ICartRepository>(new CartRepository(connectionString));
			});
		}
	}
}