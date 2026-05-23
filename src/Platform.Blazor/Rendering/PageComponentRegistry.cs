using Platform.Blazor.Components.Storefront;

namespace Platform.Blazor.Rendering;

public static class PageComponentRegistry
{
    private static readonly IReadOnlyDictionary<string, Type> ComponentTypes =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["hero"] = typeof(HeroSection),
            ["text"] = typeof(TextBlockSection),
            ["image"] = typeof(ImageSection),
            ["logo"] = typeof(LogoSection),
            ["productGrid"] = typeof(ProductGridSection),
            ["categoryGrid"] = typeof(CategoryGridSection),
            ["button"] = typeof(ButtonSection),
            ["banner"] = typeof(BannerSection),
            ["featuredProducts"] = typeof(FeaturedProductsSection),
            ["navBar"] = typeof(NavBarSection),
            ["featureGrid"] = typeof(FeatureGridSection),
            ["testimonials"] = typeof(TestimonialsSection),
            ["pricing"] = typeof(PricingSection),
            ["footer"] = typeof(FooterSection),
            ["container"] = typeof(ContainerSection),
            ["columns"] = typeof(ColumnsSection),
            ["spacer"] = typeof(SpacerSection),
            ["input"] = typeof(InputSection),
            ["form"] = typeof(FormSection),
            ["customHtml"] = typeof(CustomHtmlSection)
        };

    public static Type? Resolve(string? componentType)
    {
        return !string.IsNullOrWhiteSpace(componentType) && ComponentTypes.TryGetValue(componentType, out var type)
            ? type
            : null;
    }
}
