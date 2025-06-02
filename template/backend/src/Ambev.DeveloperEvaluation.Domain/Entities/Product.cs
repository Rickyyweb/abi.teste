using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation.Product;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Representa um produto vendável, com nome e preço.
    /// Contém validações de entrada para garantir consistência.
    /// </summary>
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        /// Construtor padrão para EF Core.
        /// </summary>
        public Product() { }

        /// <summary>
        /// Cria um novo produto com nome e preço.
        /// </summary>
        public Product(string name, decimal price)
        {
            Id = Guid.NewGuid();
            Update(name, price);
        }

        /// <summary>
        /// Atualiza o nome e o preço do produto, validando os valores.
        /// </summary>
        public void Update(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(name));

            if (price < 0)
                throw new ArgumentException("O preço do produto não pode ser negativo.", nameof(price));

            Name = name.Trim();
            Price = price;
        }

        public ValidationResultDetail Validate()
        {
            var validator = new ProductValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }
}
