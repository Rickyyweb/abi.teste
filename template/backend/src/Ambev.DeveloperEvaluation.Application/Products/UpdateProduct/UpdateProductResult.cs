namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    /// <summary>
    /// Resultado simplificado de um Update (poderia retornar dados de auditoria, mas aqui só retorna o Id).
    /// </summary>
    public record UpdateProductResult(Guid Id);
}
