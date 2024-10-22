using Ardalis.GuardClauses;
using CartService.Application.Interfaces;
using CartService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartService.WebApi.IntegrationTests.Common
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		private string _dbPathOriginal;
		private string _dbPath;

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			var basePath = Path.GetDirectoryName(typeof(TStartup).Assembly.Location);
			Guard.Against.Null(basePath, "Project root could not be located.");

			builder.ConfigureAppConfiguration((context, config) =>
			{
				var builtConfig = config.Build();
				var connectionString = builtConfig.GetConnectionString("DefaultConnection");
				Guard.Against.Null(connectionString, "Original Connection string 'DefaultConnection' not found.");

				var filename = GetFileNameFromConnectionString(connectionString);
				_dbPathOriginal = Path.Combine(basePath, filename);

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
				//Remove existing ICartRepository registration
				services.RemoveAll<ICartRepository>();

				var serviceProvider = services.BuildServiceProvider();
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();
				var connectionString = configuration.GetConnectionString("DefaultConnection");

				Guard.Against.Null(connectionString, "Connection string 'DefaultConnection' not found.");
				connectionString = string.Format(connectionString, Guid.NewGuid());
				services.AddSingleton<ICartRepository>(new CartRepository(connectionString));

				var filename = GetFileNameFromConnectionString(connectionString);
				_dbPath = Path.Combine(basePath, filename);
			});
		}

		private string GetFileNameFromConnectionString(string connectionString)
		{
			var filenameStart = connectionString.IndexOf("Filename=") + "Filename=".Length;
			var filenameEnd = connectionString.IndexOf(';', filenameStart);
			var filename = connectionString.Substring(filenameStart, filenameEnd - filenameStart);

			return filename;
		}

		public string GetDbPath() => _dbPath;
		public string GetDbPathOriginal() => _dbPathOriginal;
	}
}
