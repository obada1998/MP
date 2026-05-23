using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Common;
using Platform.Application.Products;
using Platform.Domain.Enums;
using Platform.Tests.Infrastructure;

namespace Platform.Tests;

public sealed class TenantIsolationTests
{
    [Fact]
    public async Task ProductService_rejects_cross_store_access()
    {
        await using var provider = TestHost.Create();
        var (ownerId, _, otherStoreId) = await TestHost.SeedTenantsAsync(provider);

        using var scope = provider.CreateScope();
        var products = scope.ServiceProvider.GetRequiredService<IProductService>();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            products.CreateProductAsync(ownerId, otherStoreId, new UpsertProductRequest
            {
                Name = "Cross store product",
                BasePrice = 10,
                Status = ProductStatus.Active
            }));
    }
}
