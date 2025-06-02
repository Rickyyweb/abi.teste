namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class IdempotencyKeyEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = "";      
        public Guid SaleId { get; set; }           
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
