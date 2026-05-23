using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Products;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class ProductService(
    ApplicationDbContext dbContext,
    IStoreAccessService storeAccessService) : IProductService
{
    private static readonly StoreMembershipRole[] CatalogEditors = [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff];

    public async Task<IReadOnlyCollection<ProductDto>> GetProductsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var products = await ProductQuery(storeId)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return products.Select(x => x.ToDto()).ToArray();
    }

    public async Task<ProductDto> GetProductAsync(string userId, Guid storeId, Guid productId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var product = await ProductQuery(storeId).AsNoTracking().SingleOrDefaultAsync(x => x.Id == productId, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");
        return product.ToDto();
    }

    public async Task<ProductDto> CreateProductAsync(string userId, Guid storeId, UpsertProductRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        await EnsureUniqueProductSlugAsync(storeId, slug, null, cancellationToken);
        await EnsureCategoriesBelongToStoreAsync(storeId, request.CategoryIds, cancellationToken);

        var product = new Product { StoreId = storeId };
        await ApplyProductAsync(product, request, slug, cancellationToken);
        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);
        return (await ProductQuery(storeId).AsNoTracking().SingleAsync(x => x.Id == product.Id, cancellationToken)).ToDto();
    }

    public async Task<ProductDto> UpdateProductAsync(string userId, Guid storeId, Guid productId, UpsertProductRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var product = await ProductQuery(storeId).SingleOrDefaultAsync(x => x.Id == productId, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        await EnsureUniqueProductSlugAsync(storeId, slug, productId, cancellationToken);
        await EnsureCategoriesBelongToStoreAsync(storeId, request.CategoryIds, cancellationToken);
        await ApplyProductAsync(product, request, slug, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return (await ProductQuery(storeId).AsNoTracking().SingleAsync(x => x.Id == product.Id, cancellationToken)).ToDto();
    }

    public async Task DeleteProductAsync(string userId, Guid storeId, Guid productId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var product = await dbContext.Products.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == productId, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductFieldDefinitionDto>> GetFieldDefinitionsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        return await dbContext.ProductFieldDefinitions
            .AsNoTracking()
            .Where(x => x.StoreId == storeId)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductFieldDefinitionDto> CreateFieldDefinitionAsync(string userId, Guid storeId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        JsonGuards.EnsureValidJson(request.OptionsJson, nameof(request.OptionsJson));
        JsonGuards.EnsureValidJson(request.ValidationRulesJson, nameof(request.ValidationRulesJson));
        if (!string.IsNullOrWhiteSpace(request.DefaultValueJson)) JsonGuards.EnsureValidJson(request.DefaultValueJson, nameof(request.DefaultValueJson));
        var key = NormalizeFieldKey(request.Key);
        if (await dbContext.ProductFieldDefinitions.AnyAsync(x => x.StoreId == storeId && x.Key == key, cancellationToken))
        {
            throw new InvalidOperationException("A product field already exists with this key.");
        }

        var field = new ProductFieldDefinition { StoreId = storeId };
        ApplyField(field, request, key);
        dbContext.ProductFieldDefinitions.Add(field);
        await dbContext.SaveChangesAsync(cancellationToken);
        return field.ToDto();
    }

    public async Task<ProductFieldDefinitionDto> UpdateFieldDefinitionAsync(string userId, Guid storeId, Guid fieldId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        JsonGuards.EnsureValidJson(request.OptionsJson, nameof(request.OptionsJson));
        JsonGuards.EnsureValidJson(request.ValidationRulesJson, nameof(request.ValidationRulesJson));
        if (!string.IsNullOrWhiteSpace(request.DefaultValueJson)) JsonGuards.EnsureValidJson(request.DefaultValueJson, nameof(request.DefaultValueJson));
        var field = await dbContext.ProductFieldDefinitions.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == fieldId, cancellationToken)
            ?? throw new KeyNotFoundException("Product field definition not found.");

        var key = NormalizeFieldKey(request.Key);
        if (await dbContext.ProductFieldDefinitions.AnyAsync(x => x.StoreId == storeId && x.Id != fieldId && x.Key == key, cancellationToken))
        {
            throw new InvalidOperationException("A product field already exists with this key.");
        }

        ApplyField(field, request, key);
        await dbContext.SaveChangesAsync(cancellationToken);
        return field.ToDto();
    }

    public async Task DeleteFieldDefinitionAsync(string userId, Guid storeId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, CatalogEditors, cancellationToken);
        var field = await dbContext.ProductFieldDefinitions.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == fieldId, cancellationToken)
            ?? throw new KeyNotFoundException("Product field definition not found.");
        var values = await dbContext.ProductCustomFieldValues.Where(x => x.StoreId == storeId && x.ProductFieldDefinitionId == fieldId).ToListAsync(cancellationToken);
        dbContext.ProductCustomFieldValues.RemoveRange(values);
        dbContext.ProductFieldDefinitions.Remove(field);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Product> ProductQuery(Guid storeId) => dbContext.Products
        .Where(x => x.StoreId == storeId)
        .Include(x => x.Images)
        .Include(x => x.ProductCategories)
        .Include(x => x.CustomFieldValues)
        .ThenInclude(x => x.FieldDefinition);

    private async Task ApplyProductAsync(Product product, UpsertProductRequest request, string slug, CancellationToken cancellationToken)
    {
        product.Name = request.Name.Trim();
        product.Slug = slug;
        product.Sku = request.Sku;
        product.Description = request.Description;
        product.BasePrice = request.BasePrice;
        product.Status = request.Status;
        product.PrimaryImageUrl = request.PrimaryImageUrl;
        product.PublishedAt = request.Status == ProductStatus.Active ? (product.PublishedAt ?? DateTimeOffset.UtcNow) : null;

        product.Images.Clear();
        foreach (var image in request.Images.OrderBy(x => x.DisplayOrder))
        {
            product.Images.Add(new ProductImage
            {
                StoreId = product.StoreId,
                ProductId = product.Id,
                Url = image.Url.Trim(),
                AltText = image.AltText,
                DisplayOrder = image.DisplayOrder
            });
        }

        product.ProductCategories.Clear();
        foreach (var categoryId in request.CategoryIds.Distinct())
        {
            product.ProductCategories.Add(new ProductCategory
            {
                StoreId = product.StoreId,
                ProductId = product.Id,
                CategoryId = categoryId
            });
        }

        await ApplyCustomFieldsAsync(product, request.CustomFieldValues, cancellationToken);
    }

    private async Task ApplyCustomFieldsAsync(Product product, IReadOnlyDictionary<Guid, string> values, CancellationToken cancellationToken)
    {
        var definitions = await dbContext.ProductFieldDefinitions
            .Where(x => x.StoreId == product.StoreId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
        var definitionIds = definitions.Select(x => x.Id).ToHashSet();

        var unknown = values.Keys.FirstOrDefault(id => !definitionIds.Contains(id));
        if (unknown != Guid.Empty)
        {
            throw new InvalidOperationException($"Product field '{unknown}' does not belong to this store.");
        }

        foreach (var required in definitions.Where(x => x.IsRequired))
        {
            if (!values.TryGetValue(required.Id, out var requiredValue) || string.IsNullOrWhiteSpace(requiredValue) || requiredValue == "null")
            {
                throw new InvalidOperationException($"Product field '{required.Label}' is required.");
            }
        }

        product.CustomFieldValues.Clear();
        foreach (var (fieldId, rawJson) in values)
        {
            product.CustomFieldValues.Add(new ProductCustomFieldValue
            {
                StoreId = product.StoreId,
                ProductId = product.Id,
                ProductFieldDefinitionId = fieldId,
                ValueJson = JsonGuards.NormalizeValueJson(rawJson)
            });
        }
    }

    private async Task EnsureCategoriesBelongToStoreAsync(Guid storeId, IReadOnlyCollection<Guid> categoryIds, CancellationToken cancellationToken)
    {
        var ids = categoryIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return;
        }

        var count = await dbContext.Categories.CountAsync(x => x.StoreId == storeId && ids.Contains(x.Id), cancellationToken);
        if (count != ids.Length)
        {
            throw new InvalidOperationException("One or more categories do not belong to this store.");
        }
    }

    private async Task EnsureUniqueProductSlugAsync(Guid storeId, string slug, Guid? excludingProductId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Products.AnyAsync(
            x => x.StoreId == storeId && x.Slug == slug && (!excludingProductId.HasValue || x.Id != excludingProductId),
            cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("A product already exists with this slug.");
        }
    }

    private static void ApplyField(ProductFieldDefinition field, UpsertProductFieldDefinitionRequest request, string key)
    {
        field.Key = key;
        field.Label = request.Label.Trim();
        field.FieldType = request.FieldType;
        field.IsRequired = request.IsRequired;
        field.IsVisibleOnListing = request.IsVisibleOnListing;
        field.IsVisibleOnProductPage = request.IsVisibleOnProductPage;
        field.IsSearchable = request.IsSearchable;
        field.IsFilterable = request.IsFilterable;
        field.DisplayOrder = request.DisplayOrder;
        field.Placeholder = request.Placeholder?.Trim();
        field.HelpText = request.HelpText?.Trim();
        field.DefaultValueJson = request.DefaultValueJson;
        field.ValidationRulesJson = string.IsNullOrWhiteSpace(request.ValidationRulesJson) ? "{}" : request.ValidationRulesJson;
        field.OptionsJson = string.IsNullOrWhiteSpace(request.OptionsJson) ? "{}" : request.OptionsJson;
    }

    private static string NormalizeFieldKey(string key)
    {
        var normalized = SlugGenerator.Create(key).Replace('-', '_');
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException("Product field key is required.");
        }

        return normalized;
    }
}
