using CartService.Application.Behaviours;
using CartService.Application.Interfaces;
using CartService.Application.UseCases.CartItems.Commands;
using CartService.Application.UseCases.CartItems.Validators;
using CartService.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CartService.IntegrationTests.Common
{
	public abstract class TestBase
	{
		protected IMediator _mediator;
		protected ICartRepository _cartRepository;
		private string _dbPath = "TestCart.db";
		public const int ValidCartId = 1;

		[SetUp]
		protected void SetUp()
        {
			// Setup the service collection
			var services = new ServiceCollection();

			// Create a unique database path for each test
			_dbPath = $"TestCart_{Guid.NewGuid()}.db";
			services.AddSingleton<ICartRepository>(new CartRepository(_dbPath));

			// Register MediatR and the handler
			services.AddMediatR(cfg => {
				cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(ICartRepository)));
				cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			});

			// Register validators
			services.AddTransient<IValidator<AddItemToCartCommand>, AddItemToCartCommandValidator>();

			// Build the service provider
			var serviceProvider = services.BuildServiceProvider();
			_mediator = serviceProvider.GetRequiredService<IMediator>();
			_cartRepository = serviceProvider.GetRequiredService<ICartRepository>();

			// Seed the database asynchronously
			SeedDatabase().GetAwaiter().GetResult();
		}

		protected async virtual Task SeedDatabase()
		{
			// Override this method in derived classes to seed test data
		}

		[TearDown]
		public void TearDown()
		{
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
		}
	}
}
