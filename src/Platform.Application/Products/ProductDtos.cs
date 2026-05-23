using Platform.Domain.Enums;

namespace Platform.Application.Products;

public sealed class ProductFieldDefinitionDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ProductFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVisibleOnListing { get; set; }
    public bool IsVisibleOnProductPage { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? DefaultValueJson { get; set; }
    public string ValidationRulesJson { get; set; } = "{}";
    public string OptionsJson { get; set; } = "{}";
}

public sealed class UpsertProductFieldDefinitionRequest
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ProductFieldType FieldType { get; set; } = ProductFieldType.Text;
    public bool IsRequired { get; set; }
    public bool IsVisibleOnListing { get; set; } = true;
    public bool IsVisibleOnProductPage { get; set; } = true;
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? DefaultValueJson { get; set; }
    public string ValidationRulesJson { get; set; } = "{}";
    public string OptionsJson { get; set; } = "{}";
}

public sealed class ProductImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class ProductCustomFieldValueDto
{
    public Guid FieldDefinitionId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ProductFieldType FieldType { get; set; }
    public string ValueJson { get; set; } = "null";
}

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public ProductStatus Status { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public IReadOnlyCollection<Guid> CategoryIds { get; set; } = [];
    public IReadOnlyCollection<ProductImageDto> Images { get; set; } = [];
    public IReadOnlyCollection<ProductCustomFieldValueDto> CustomFields { get; set; } = [];
}

public sealed class UpsertProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Sku { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public string? PrimaryImageUrl { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public IReadOnlyCollection<Guid> CategoryIds { get; set; } = [];
    public IReadOnlyCollection<ProductImageDto> Images { get; set; } = [];
    public Dictionary<Guid, string> CustomFieldValues { get; set; } = [];
}
