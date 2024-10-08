using CatalogService.Application.Behaviours;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Categories.Commands;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Application.UseCases.Categories.Validators;
using CatalogService.Application.UseCases.Products.Commands;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Application.UseCases.Products.Validators;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CatalogService.Application.IntegrationTests.Common
{
	public abstract class TestBase : IDisposable
    {
        protected readonly IApplicationDbContext _context;
		protected readonly IMediator _mediator;

		protected TestBase()
        {
            var services = new ServiceCollection();

            // Configure in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Register IApplicationDbContext
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            // Register AutoMapper
            services.AddAutoMapper(typeof(IApplicationDbContext));

            // Register Mediator
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(IApplicationDbContext)));
				cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
			});

			// Configure AutoMapper
			services.AddAutoMapper(cfg =>
			{
				cfg.CreateMap<CategoryDto, Category>();
				cfg.CreateMap<ProductDto, Product>();
			});

			// Register validators
			services.AddTransient<IValidator<CreateCategoryCommnad>, CreateCategoryCommandValidator>();
			services.AddTransient<IValidator<GetCategoriesQuery>, GetCategoriesQueryValidator>();
			services.AddTransient<IValidator<UpdateCategoryCommnad>, UpdateCategoryCommandValidator>();

			services.AddTransient<IValidator<CreateProductCommnad>, CreateProductCommandValidator>();
			services.AddTransient<IValidator<GetProductsQuery>, GetProductsQueryValidator>();
			services.AddTransient<IValidator<UpdateProductCommnad>, UpdateProductCommandValidator>();

			// Build service provider
			var serviceProvider = services.BuildServiceProvider();

            _context = serviceProvider.GetRequiredService<IApplicationDbContext>();
			_mediator = serviceProvider.GetRequiredService<IMediator>();

			// Seed the database asynchronously
			SeedDatabase().GetAwaiter().GetResult();
		}

        protected async virtual Task SeedDatabase()
        {
            // Override this method in derived classes to seed test data
        }

        public void Dispose()
        {
            // Cleanup
            var dbContext = (ApplicationDbContext)_context;
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
    }
}