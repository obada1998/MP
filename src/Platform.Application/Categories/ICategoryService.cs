namespace Platform.Application.Categories;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(string userId, Guid storeId, UpsertCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateCategoryAsync(string userId, Guid storeId, Guid categoryId, UpsertCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(string userId, Guid storeId, Guid categoryId, CancellationToken cancellationToken = default);
}
