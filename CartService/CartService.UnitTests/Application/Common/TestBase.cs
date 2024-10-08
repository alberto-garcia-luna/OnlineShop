using CartService.Application.Behaviours;
using CartService.Application.Interfaces;
using CartService.Application.UseCases.CartItems.Commands;
using CartService.Application.UseCases.CartItems.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace CartService.UnitTests.Application.Common
{
    public abstract class TestBase
    {
        protected readonly IMediator _mediator;
        protected readonly Mock<ICartRepository> _mockRepository;
        public const int ValidCartId = 1;

        protected TestBase()
        {
            // Setup the service collection
            var services = new ServiceCollection();
            _mockRepository = new Mock<ICartRepository>();

            // Register MediatR and the handler
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(ICartRepository)));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            // Register validators
            services.AddTransient<IValidator<AddItemToCartCommand>, AddItemToCartCommandValidator>();

            services.AddSingleton(_mockRepository.Object);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            _mediator = serviceProvider.GetRequiredService<IMediator>();

            SeedDatabase();
        }

        protected virtual void SeedDatabase()
        {
            // Override this method in derived classes to seed test data
        }
    }
}
