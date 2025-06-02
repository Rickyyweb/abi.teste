using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    /// <summary>
    /// Configuração de EF Core para a entidade Product.
    /// Faz o mapeamento de propriedades e o seed inicial.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            // Seed inicial de 3 produtos
            builder.HasData(
                new Product("Produto A", 10.50m) { Id = Guid.Parse("6f1e4f02-1a35-4b2d-9fdd-3a7c5ab1ae2b") },
                new Product("Produto B", 25.00m) { Id = Guid.Parse("b254a3d4-7c76-4e14-af08-2dcf928d3b10") },
                new Product("Produto C", 7.99m) { Id = Guid.Parse("d39c8c5e-5b92-4b28-9f77-8c3d2e91f5f4") }
            );
        }
    }
}
