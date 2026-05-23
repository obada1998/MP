using Platform.Application.Rendering;

namespace Platform.Tests;

public sealed class PageComponentCatalogTests
{
    [Theory]
    [InlineData("hero")]
    [InlineData("productGrid")]
    [InlineData("customHtml")]
    public void Catalog_resolves_known_component_types(string type)
    {
        var definition = PageComponentCatalog.Get(type);

        Assert.Equal(type, definition.Type);
        Assert.True(PageComponentCatalog.IsKnown(type));
    }
}
