namespace Platform.Application.Products;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductDto>> GetProductsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<ProductDto> GetProductAsync(string userId, Guid storeId, Guid productId, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(string userId, Guid storeId, UpsertProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateProductAsync(string userId, Guid storeId, Guid productId, UpsertProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(string userId, Guid storeId, Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductFieldDefinitionDto>> GetFieldDefinitionsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<ProductFieldDefinitionDto> CreateFieldDefinitionAsync(string userId, Guid storeId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken = default);
    Task<ProductFieldDefinitionDto> UpdateFieldDefinitionAsync(string userId, Guid storeId, Guid fieldId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken = default);
    Task DeleteFieldDefinitionAsync(string userId, Guid storeId, Guid fieldId, CancellationToken cancellationToken = default);
}
