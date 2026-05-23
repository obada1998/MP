using System.Text.Json;
using Platform.Application.Categories;
using Platform.Application.Orders;
using Platform.Application.Pages;
using Platform.Application.Products;
using Platform.Application.Stores;
using Platform.Domain.Entities;

namespace Platform.Infrastructure.Services;

internal static class DtoMappers
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static StoreDto ToDto(this Store store) => new()
    {
        Id = store.Id,
        Name = store.Name,
        Slug = store.Slug,
        LogoUrl = store.LogoUrl,
        Domain = store.Domain,
        ThemeName = store.ThemeName,
        IsActive = store.IsActive
    };

    public static StoreSettingsDto ToDto(this StoreSettings settings) => new()
    {
        StoreId = settings.StoreId,
        Currency = settings.Currency,
        Culture = settings.Culture,
        ContactEmail = settings.ContactEmail,
        SettingsJson = settings.SettingsJson
    };

    public static ThemeSettingsDto ToDto(this ThemeSettings settings) => new()
    {
        StoreId = settings.StoreId,
        ThemeName = settings.ThemeName,
        PrimaryColor = settings.PrimaryColor,
        AccentColor = settings.AccentColor,
        FontFamily = settings.FontFamily,
        CustomCss = settings.CustomCss,
        SettingsJson = settings.SettingsJson
    };

    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        StoreId = category.StoreId,
        Name = category.Name,
        Slug = category.Slug,
        Description = category.Description,
        ParentCategoryId = category.ParentCategoryId,
        IsActive = category.IsActive,
        SortOrder = category.SortOrder,
        ProductCount = category.ProductCategories.Count
    };

    public static ProductFieldDefinitionDto ToDto(this ProductFieldDefinition field) => new()
    {
        Id = field.Id,
        StoreId = field.StoreId,
        Key = field.Key,
        Label = field.Label,
        FieldType = field.FieldType,
        IsRequired = field.IsRequired,
        IsVisibleOnListing = field.IsVisibleOnListing,
        IsVisibleOnProductPage = field.IsVisibleOnProductPage,
        IsSearchable = field.IsSearchable,
        IsFilterable = field.IsFilterable,
        DisplayOrder = field.DisplayOrder,
        Placeholder = field.Placeholder,
        HelpText = field.HelpText,
        DefaultValueJson = field.DefaultValueJson,
        ValidationRulesJson = field.ValidationRulesJson,
        OptionsJson = field.OptionsJson
    };

    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        StoreId = product.StoreId,
        Name = product.Name,
        Slug = product.Slug,
        Sku = product.Sku,
        Description = product.Description,
        BasePrice = product.BasePrice,
        Status = product.Status,
        PrimaryImageUrl = product.PrimaryImageUrl,
        PublishedAt = product.PublishedAt,
        CategoryIds = product.ProductCategories.Select(x => x.CategoryId).ToArray(),
        Images = product.Images.OrderBy(x => x.DisplayOrder).Select(x => new ProductImageDto
        {
            Id = x.Id,
            Url = x.Url,
            AltText = x.AltText,
            DisplayOrder = x.DisplayOrder
        }).ToArray(),
        CustomFields = product.CustomFieldValues
            .OrderBy(x => x.FieldDefinition.DisplayOrder)
            .Select(x => new ProductCustomFieldValueDto
            {
                FieldDefinitionId = x.ProductFieldDefinitionId,
                Key = x.FieldDefinition.Key,
                Label = x.FieldDefinition.Label,
                FieldType = x.FieldDefinition.FieldType,
                ValueJson = x.ValueJson
            })
            .ToArray()
    };

    public static PageDto ToDto(this Page page) => new()
    {
        Id = page.Id,
        StoreId = page.StoreId,
        Title = page.Title,
        Slug = page.Slug,
        IsHomePage = page.IsHomePage,
        IsPublished = page.IsPublished,
        PublishedAt = page.PublishedAt
    };

    public static PageLayoutDto ToDto(this PageLayout layout) => new()
    {
        Id = layout.Id,
        PageId = layout.PageId,
        StoreId = layout.StoreId,
        Version = layout.Version,
        Status = layout.Status,
        LayoutJson = layout.LayoutJson,
        PublishedAt = layout.PublishedAt,
        Layout = JsonSerializer.Deserialize<PageLayoutDocument>(layout.LayoutJson, JsonOptions) ?? new PageLayoutDocument()
    };

    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        StoreId = order.StoreId,
        OrderNumber = order.OrderNumber,
        CustomerEmail = order.CustomerEmail,
        CustomerName = order.CustomerName,
        Status = order.Status,
        Subtotal = order.Subtotal,
        Tax = order.Tax,
        Shipping = order.Shipping,
        Total = order.Total,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(x => new OrderItemDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductName = x.ProductName,
            Sku = x.Sku,
            UnitPrice = x.UnitPrice,
            Quantity = x.Quantity,
            LineTotal = x.LineTotal
        }).ToArray()
    };
}
