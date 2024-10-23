using CartService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.WebApi.IntegrationTests.Common
{
	public class TestBase
	{
		private CustomWebApplicationFactory<Program> _factory;
		protected HttpClient _client;
		private ICartRepository _cartRepository;

		public const string ValidCartId = "CartUniqueKey-1";

		[SetUp]
		public virtual void SetUp()
		{
			_factory = new CustomWebApplicationFactory<Program>();
			_client = _factory.CreateClient();

			var scope = _factory.Services.CreateScope();
			var serviceProvider = scope.ServiceProvider;
			_cartRepository = serviceProvider.GetRequiredService<ICartRepository>();

			// Seed the database asynchronously
			SeedDatabase().GetAwaiter().GetResult();
		}

		[TearDown]
		public virtual void TearDown()
		{
			_cartRepository.CleanDatabase().GetAwaiter().GetResult();

			// Dispose of the cart repository if needed
			if (_cartRepository is IDisposable disposable)
			{
				disposable.Dispose();
			}

			_client?.Dispose();
			_factory?.Dispose();
		}

		protected async virtual Task SeedDatabase()
		{
			// Override this method in derived classes to seed test data
		}
	}
}