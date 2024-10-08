using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configuration
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(t => t.Id)
				.ValueGeneratedOnAdd();

			builder.Property(t => t.Name)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(t => t.CategoryId)
				.IsRequired();

			builder.Property(t => t.Price)
				.IsRequired();

			builder.Property(t => t.Amount)
				.IsRequired();
		}
	}
}
