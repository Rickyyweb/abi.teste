using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    /// <summary>
    /// Configuração de EF Core para a entidade Sale.
    /// Mapeia a tabela Sales e propriedades.
    /// </summary>
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.CustomerId)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.Subtotal)
                   .HasColumnType("decimal(18,2)");

            builder.Property(s => s.Discount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(s => s.TotalAmount)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
