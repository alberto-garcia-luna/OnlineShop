using CartService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

[assembly: Parallelizable(ParallelScope.None)]
namespace CartService.WebApi.IntegrationTests.Common
{
	public class TestBase
	{
		private CustomWebApplicationFactory<Program> _factory;
		protected HttpClient _client;
		private ICartRepository _cartRepository;

		private string _dbPath;
		private string _dbPathOriginal;
		public const int ValidCartId = 1;

		[SetUp]
		public virtual void SetUp()
		{
			_factory = new CustomWebApplicationFactory<Program>();
			_client = _factory.CreateClient();
			_dbPathOriginal = _factory.GetDbPathOriginal();
			_dbPath = _factory.GetDbPath();

			var scope = _factory.Services.CreateScope();
			var serviceProvider = scope.ServiceProvider;
			_cartRepository = serviceProvider.GetRequiredService<ICartRepository>();

			// Seed the database asynchronously
			SeedDatabase().GetAwaiter().GetResult();
		}

		[TearDown]
		public virtual void TearDown()
		{
			//_cartRepository.CleanDatabase().GetAwaiter().GetResult();
			_client?.Dispose();

			// Dispose of the cart repository if needed
			if (_cartRepository is IDisposable disposable)
			{
				disposable.Dispose();
			}

			// Clean up the database file after each test
			if (File.Exists(_dbPath))
			{
				File.Delete(_dbPath);
			}

			_factory?.Dispose();
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			_factory?.Dispose();

			//if (File.Exists(_dbPathOriginal))
			//{
			//	File.Delete(_dbPathOriginal);
			//}
		}

		protected async virtual Task SeedDatabase()
		{
			// Override this method in derived classes to seed test data
		}
	}
}
