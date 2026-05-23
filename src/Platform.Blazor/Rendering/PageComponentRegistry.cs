using Platform.Blazor.Components.Storefront;

namespace Platform.Blazor.Rendering;

public static class PageComponentRegistry
{
    private static readonly IReadOnlyDictionary<string, Type> ComponentTypes =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["hero"] = typeof(HeroSection),
            ["textBlock"] = typeof(TextBlockSection),
            ["heading"] = typeof(TextBlockSection),
            ["paragraph"] = typeof(TextBlockSection),
            ["text"] = typeof(TextBlockSection),
            ["image"] = typeof(ImageSection),
            ["imageGallery"] = typeof(ImageSection),
            ["videoEmbed"] = typeof(ImageSection),
            ["icon"] = typeof(LogoSection),
            ["logo"] = typeof(LogoSection),
            ["productGrid"] = typeof(ProductGridSection),
            ["productList"] = typeof(ProductGridSection),
            ["productCard"] = typeof(ProductGridSection),
            ["productDetails"] = typeof(ProductGridSection),
            ["categoryGrid"] = typeof(CategoryGridSection),
            ["categoryList"] = typeof(CategoryGridSection),
            ["button"] = typeof(ButtonSection),
            ["addToCartButton"] = typeof(ButtonSection),
            ["divider"] = typeof(SpacerSection),
            ["box"] = typeof(ContainerSection),
            ["banner"] = typeof(BannerSection),
            ["header"] = typeof(NavBarSection),
            ["navigationMenu"] = typeof(NavBarSection),
            ["featuredProducts"] = typeof(FeaturedProductsSection),
            ["featuredProduct"] = typeof(FeaturedProductsSection),
            ["navBar"] = typeof(NavBarSection),
            ["gridContainer"] = typeof(ContainerSection),
            ["twoColumnSection"] = typeof(ColumnsSection),
            ["threeColumnSection"] = typeof(ColumnsSection),
            ["featureGrid"] = typeof(FeatureGridSection),
            ["testimonials"] = typeof(TestimonialsSection),
            ["pricing"] = typeof(PricingSection),
            ["footer"] = typeof(FooterSection),
            ["container"] = typeof(ContainerSection),
            ["columns"] = typeof(ColumnsSection),
            ["spacer"] = typeof(SpacerSection),
            ["input"] = typeof(InputSection),
            ["form"] = typeof(FormSection),
            ["contactForm"] = typeof(FormSection),
            ["newsletterForm"] = typeof(FormSection),
            ["searchBox"] = typeof(InputSection),
            ["customHtml"] = typeof(CustomHtmlSection),
            ["json"] = typeof(JsonSection)
        };

    public static Type? Resolve(string? componentType)
    {
        return !string.IsNullOrWhiteSpace(componentType) && ComponentTypes.TryGetValue(componentType, out var type)
            ? type
            : null;
    }
}
