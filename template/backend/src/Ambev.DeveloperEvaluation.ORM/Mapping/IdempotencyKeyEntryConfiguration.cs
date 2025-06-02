using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class IdempotencyKeyEntryConfiguration : IEntityTypeConfiguration<IdempotencyKeyEntry>
    {
        public void Configure(EntityTypeBuilder<IdempotencyKeyEntry> builder)
        {
            builder.ToTable("IdempotencyKeys");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SaleId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.HasIndex(x => x.Key).IsUnique();
        }
    }
}
