using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Persistence
{
    // <summary>
    /// DbContext que agrupa Products, Sales e SaleItems.
    /// Aplica as configurações de mapeamento definidas em Mapping/.
    /// </summary>
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<IdempotencyKeyEntry> IdempotencyKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica cada configuração em Mapping/
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new SaleConfiguration());
            modelBuilder.ApplyConfiguration(new SaleItemConfiguration());
            modelBuilder.ApplyConfiguration(new IdempotencyKeyEntryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
