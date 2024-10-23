using AutoMapper;
using CartService.Application.Interfaces;
using CartService.Application.UseCases.CartItems.Queries;
using CartService.Domain.Entities;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CartService.UnitTests.Application.Mappings
{
	[TestFixture]
	public class MappingTests
	{
		private readonly IConfigurationProvider _configuration;
		private readonly IMapper _mapper;

		public MappingTests()
		{
			_configuration = new MapperConfiguration(config =>
				config.AddMaps(Assembly.GetAssembly(typeof(ICartRepository))));

			_mapper = _configuration.CreateMapper();
		}

		[Test]
		public void ShouldHaveValidConfiguration()
		{
			_configuration.AssertConfigurationIsValid();
		}

		[Test]
		[TestCase(typeof(CartItem), typeof(CartItemDto))]
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
