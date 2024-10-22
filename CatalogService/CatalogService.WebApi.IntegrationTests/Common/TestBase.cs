using CatalogService.Application.Common.Interfaces;
using CatalogService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.WebApi.IntegrationTests.Common
{
	public abstract class TestBase
	{
		protected HttpClient _client;
		protected IApplicationDbContext _context;
		private CustomWebApplicationFactory<Program> _factory;

        protected TestBase()
        {
			_factory = new CustomWebApplicationFactory<Program>();
		}

        [SetUp]
		public virtual void SetUp()
		{
			InitializeContext();
		}

		[TearDown]
		public virtual void TearDown()
		{
			if (_context != null)
			{
				var dbContext = (ApplicationDbContext)_context;
				dbContext.Database.EnsureDeleted();
				dbContext.Dispose();
			}

			_client?.Dispose();
		}

		[OneTimeTearDown]
		public virtual void OneTimeTearDown()
		{
			_factory?.Dispose();
		}

		private void InitializeContext()
		{			
			_client = _factory.CreateClient();
			ReloadContext();
		}

		protected void ReloadContext()
		{
			var scope = _factory.Services.CreateScope();
			_context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
		}

		protected async Task ReloadEntityAsync<TEntity>(TEntity entity) where TEntity : class
		{
			if (entity != null)
			{
				var dbContext = (ApplicationDbContext)_context;
				await dbContext.Entry(entity).ReloadAsync();
			}
		}
	}
}
