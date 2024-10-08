using AutoMapper;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.UseCases.Categories.Queries;
using CatalogService.Application.UseCases.Products.Queries;
using CatalogService.Domain.Entities;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CatalogService.Application.UnitTests.Mappings
{
	[TestFixture]
	public class MappingTests
	{
		private readonly IConfigurationProvider _configuration;
		private readonly IMapper _mapper;

		public MappingTests()
		{
			_configuration = new MapperConfiguration(config =>
				config.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));

			_mapper = _configuration.CreateMapper();
		}

		[Test]
		public void ShouldHaveValidConfiguration()
		{
			_configuration.AssertConfigurationIsValid();
		}

		[Test]
		[TestCase(typeof(Category), typeof(CategoryDto))]
		[TestCase(typeof(Product), typeof(ProductDto))]
		public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
		{
			var instance = GetInstanceOf(source);

			_mapper.Map(instance, source, destination);
		}

		private object GetInstanceOf(Type type)
		{
			if (type.GetConstructor(Type.EmptyTypes) != null)
				return Activator.CreateInstance(type)!;

			// Type without parameterless constructor
			return RuntimeHelpers.GetUninitializedObject(type);
		}
	}
}
