using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Products;
using Platform.Domain.Enums;
using Platform.Tests.Infrastructure;

namespace Platform.Tests;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task ProductService_creates_product_with_store_defined_custom_fields()
    {
        await using var provider = TestHost.Create();
        var (ownerId, storeId, _) = await TestHost.SeedTenantsAsync(provider);

        using var scope = provider.CreateScope();
        var products = scope.ServiceProvider.GetRequiredService<IProductService>();
        var field = await products.CreateFieldDefinitionAsync(ownerId, storeId, new UpsertProductFieldDefinitionRequest
        {
            Key = "color",
            Label = "Color",
            FieldType = ProductFieldType.Color,
            IsRequired = true
        });

        var product = await products.CreateProductAsync(ownerId, storeId, new UpsertProductRequest
        {
            Name = "Metadata Product",
            BasePrice = 19.95m,
            Status = ProductStatus.Active,
            CustomFieldValues = new Dictionary<Guid, string>
            {
                [field.Id] = "\"#111827\""
            }
        });

        Assert.Equal(storeId, product.StoreId);
        Assert.Single(product.CustomFields);
        Assert.Equal("color", product.CustomFields.Single().Key);
        Assert.Equal("\"#111827\"", product.CustomFields.Single().ValueJson);
    }
}
