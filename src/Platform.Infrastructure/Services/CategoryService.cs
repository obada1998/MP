using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Categories;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class CategoryService(
    ApplicationDbContext dbContext,
    IStoreAccessService storeAccessService) : ICategoryService
{
    private static readonly StoreMembershipRole[] CatalogEditors = [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff];

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Include(x => x.ProductCategories)
            .Where(x => x.StoreId == storeId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return categories.Select(x => x.ToDto()).ToArray();
    }

    public async Task<CategoryDto> CreateCategoryAsync(string userId, Guid storeId, UpsertCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        await EnsureUniqueSlugAsync(storeId, slug, null, cancellationToken);
        await EnsureParentBelongsToStoreAsync(storeId, request.ParentCategoryId, cancellationToken);

        var category = new Category { StoreId = storeId };
        Apply(category, request, slug);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateCategoryAsync(string userId, Guid storeId, Guid categoryId, UpsertCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var category = await dbContext.Categories
            .Include(x => x.ProductCategories)
            .SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");

        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        await EnsureUniqueSlugAsync(storeId, slug, categoryId, cancellationToken);
        await EnsureParentBelongsToStoreAsync(storeId, request.ParentCategoryId, cancellationToken);
        if (request.ParentCategoryId == categoryId)
        {
            throw new InvalidOperationException("A category cannot be its own parent.");
        }

        Apply(category, request, slug);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category.ToDto();
    }

    public async Task DeleteCategoryAsync(string userId, Guid storeId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var category = await dbContext.Categories.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        var productCategories = await dbContext.ProductCategories
            .Where(x => x.StoreId == storeId && x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
        dbContext.ProductCategories.RemoveRange(productCategories);
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Apply(Category category, UpsertCategoryRequest request, string slug)
    {
        category.Name = request.Name.Trim();
        category.Slug = slug;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.IsActive = request.IsActive;
        category.SortOrder = request.SortOrder;
    }

    private async Task EnsureUniqueSlugAsync(Guid storeId, string slug, Guid? excludingCategoryId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Categories.AnyAsync(
            x => x.StoreId == storeId && x.Slug == slug && (!excludingCategoryId.HasValue || x.Id != excludingCategoryId),
            cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("A category already exists with this slug.");
        }
    }

    private async Task EnsureParentBelongsToStoreAsync(Guid storeId, Guid? parentCategoryId, CancellationToken cancellationToken)
    {
        if (!parentCategoryId.HasValue)
        {
            return;
        }

        var exists = await dbContext.Categories.AnyAsync(x => x.StoreId == storeId && x.Id == parentCategoryId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("Parent category does not belong to this store.");
        }
    }
}
