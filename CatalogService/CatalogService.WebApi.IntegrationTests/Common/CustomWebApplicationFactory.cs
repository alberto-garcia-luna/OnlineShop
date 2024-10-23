using CatalogService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CatalogService.WebApi.IntegrationTests.Common
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestServices(services =>
			{
				services
					.RemoveAll<DbContextOptions<ApplicationDbContext>>()
					.AddDbContext<ApplicationDbContext>(options =>
						options.UseInMemoryDatabase("TestDatabase"));
			});
		}
	}
}
