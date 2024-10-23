using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace CartService.WebApi
{
	public static class DependencyInjection
	{
		public static void AddServices(this IServiceCollection services)
		{
			services.AddControllers();

			// Add API versioning
			services.AddApiVersioning(options =>
			{
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ReportApiVersions = true;
			})
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
			});

			// Configure Swagger
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(c =>
			{
				var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
				foreach (var description in provider.ApiVersionDescriptions)
				{
					c.SwaggerDoc(description.GroupName, new OpenApiInfo
					{
						Title = "Cart Service API",
						Version = description.ApiVersion.ToString(),
						Description = "API for managing cart items"
					});
				}
			});
		}

		public static void ConfigureMiddleware(this WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
					{
						options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
							description.GroupName.ToUpperInvariant());
					}
				});
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
		}
	}
}