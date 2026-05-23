using System.Text.Json;
using System.Text.Json.Nodes;

namespace Platform.Application.Pages;

public sealed class PageTemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ThumbnailClass { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#0f766e";
    public IReadOnlyCollection<string> Devices { get; set; } = ["Desktop", "Tablet", "Mobile"];
    public PageLayoutDocument Layout { get; set; } = new();
}

public static class PageTemplateCatalog
{
    public const string BlankTemplateId = "blank-page";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IReadOnlyCollection<string> Categories { get; } =
    [
        "Fashion",
        "Electronics",
        "Beauty",
        "Home & Living",
        "Food & Beverage",
        "Jewelry",
        "Sports",
        "Digital Products",
        "Blank Page"
    ];

    public static IReadOnlyCollection<PageTemplateDto> All { get; } =
    [
        FashionTemplate(),
        ElectronicsTemplate(),
        BeautyTemplate(),
        HomeTemplate(),
        FoodTemplate(),
        JewelryTemplate(),
        SportsTemplate(),
        DigitalProductsTemplate(),
        BlankTemplate()
    ];

    public static PageTemplateDto Get(string templateId)
    {
        return All.SingleOrDefault(template => template.Id.Equals(templateId, StringComparison.OrdinalIgnoreCase))
            ?? All.Single(template => template.Id == BlankTemplateId);
    }

    public static PageLayoutDocument CreateLayout(string templateId, string pageId)
    {
        var template = Get(templateId);
        var json = JsonSerializer.Serialize(template.Layout, JsonOptions);
        var layout = JsonSerializer.Deserialize<PageLayoutDocument>(json, JsonOptions) ?? new PageLayoutDocument();
        layout.PageId = pageId;
        AssignFreshIds(layout.Sections);
        return layout;
    }

    private static PageTemplateDto FashionTemplate() => Template(
        "fashion-boutique",
        "Fashion Boutique",
        "A premium apparel homepage with editorial hero, collections, bestsellers, offer banner, reviews, and email capture.",
        "Fashion",
        "template-fashion",
        "#be123c",
        CommerceLayout(
            brand: "Aster Apparel",
            heroTitle: "Elevated layers for every day",
            heroSubtitle: "Soft tailoring, refined basics, and limited seasonal drops for modern wardrobes.",
            imageUrl: "https://images.unsplash.com/photo-1445205170230-053b83016050?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Shop the edit",
            featureItems: [
                ("New arrivals", "Fresh silhouettes, premium fabrics, and weekly seasonal releases."),
                ("Core essentials", "Everyday pieces made to style across work, travel, and weekends."),
                ("Limited capsule", "Small-batch drops with richer textures and exclusive colorways.")
            ],
            bannerTitle: "Free shipping over $75",
            bannerSubtitle: "Offer applies automatically at checkout on all domestic orders this week.",
            testimonialOne: ("The fabric quality feels boutique without the boutique markup.", "Lena W.", "Verified customer"),
            testimonialTwo: ("The sizing notes made online shopping easy.", "Mira K.", "Repeat buyer")));

    private static PageTemplateDto ElectronicsTemplate() => Template(
        "electronics-store",
        "Electronics Store",
        "A sharp tech commerce homepage for devices, accessories, specs, product grids, and support conversion.",
        "Electronics",
        "template-electronics",
        "#2563eb",
        CommerceLayout(
            brand: "VoltHaus",
            heroTitle: "Smarter gear for connected work",
            heroSubtitle: "Curated laptops, audio, accessories, and desk tech tested for daily productivity.",
            imageUrl: "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Built for better setups",
            featureItems: [
                ("Workstation bundles", "Docking, displays, keyboards, and accessories that work together."),
                ("Portable power", "Chargers, cables, and travel-ready essentials for flexible work."),
                ("Expert picks", "Every product is selected for reliability, support, and warranty coverage.")
            ],
            bannerTitle: "Launch-week accessory bundle",
            bannerSubtitle: "Add any laptop or tablet and save 20 percent on compatible accessories.",
            testimonialOne: ("The buying guides helped us outfit the whole team quickly.", "Owen P.", "Operations Lead"),
            testimonialTwo: ("Everything arrived configured and ready to use.", "Nadia S.", "Small business owner")));

    private static PageTemplateDto BeautyTemplate() => Template(
        "beauty-skincare",
        "Beauty & Skincare",
        "A warm skincare storefront with routines, product education, reviews, and subscription capture.",
        "Beauty",
        "template-beauty",
        "#db2777",
        CommerceLayout(
            brand: "Luma Skin",
            heroTitle: "Daily skincare with a lighter touch",
            heroSubtitle: "Clean routines for hydration, barrier support, and a simple morning-to-night ritual.",
            imageUrl: "https://images.unsplash.com/photo-1596462502278-27bfdc403348?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Find your routine",
            featureItems: [
                ("Hydration", "Lightweight serums and creams for resilient daily moisture."),
                ("Sensitive skin", "Fragrance-conscious formulas selected for calm, balanced care."),
                ("Refill program", "Subscribe to essentials and reduce packaging waste.")
            ],
            bannerTitle: "Free cleanser with routine sets",
            bannerSubtitle: "Build a three-step routine and receive a travel-size cleanser.",
            testimonialOne: ("The routine builder made it easy to choose the right products.", "Amira C.", "Customer"),
            testimonialTwo: ("My reorder reminders arrive exactly when I need them.", "Jules R.", "Subscriber")));

    private static PageTemplateDto HomeTemplate() => Template(
        "home-decor-store",
        "Home Decor Store",
        "A refined home goods layout for collections, rooms, featured products, bundles, and inspiration.",
        "Home & Living",
        "template-home",
        "#0f766e",
        CommerceLayout(
            brand: "Hearth & Form",
            heroTitle: "Objects that make rooms feel finished",
            heroSubtitle: "Lighting, textiles, ceramics, and storage pieces chosen for calmer everyday spaces.",
            imageUrl: "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Curated by room",
            featureItems: [
                ("Living room", "Layered textiles, lighting, and side tables for relaxed gathering."),
                ("Kitchen", "Durable ceramics, linens, and useful countertop pieces."),
                ("Bedroom", "Soft textures and quiet storage for a better end-of-day routine.")
            ],
            bannerTitle: "Room refresh bundles",
            bannerSubtitle: "Save on coordinated sets designed by our in-house styling team.",
            testimonialOne: ("The product photos made it easy to picture pieces in our home.", "Claire T.", "Customer"),
            testimonialTwo: ("The bundle looked intentional without needing a designer.", "Sam H.", "Customer")));

    private static PageTemplateDto FoodTemplate() => Template(
        "artisan-food",
        "Food & Beverage",
        "A rich specialty food storefront for bundles, subscriptions, tasting notes, reviews, and gift capture.",
        "Food & Beverage",
        "template-food",
        "#b45309",
        CommerceLayout(
            brand: "Pantry House",
            heroTitle: "Small-batch pantry staples worth sharing",
            heroSubtitle: "Olive oils, preserves, coffee, and gift boxes sourced from independent makers.",
            imageUrl: "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Shop by occasion",
            featureItems: [
                ("Breakfast table", "Coffee, jam, honey, and baked-goods pairings for better mornings."),
                ("Dinner gifts", "Host-ready oils, salts, sauces, and thoughtful finishing touches."),
                ("Monthly pantry", "A rotating subscription of seasonal small-batch goods.")
            ],
            bannerTitle: "Build a gift box",
            bannerSubtitle: "Choose any four pantry favorites and we will package them for gifting.",
            testimonialOne: ("The gift box felt personal and arrived beautifully packed.", "Maya D.", "Gift buyer"),
            testimonialTwo: ("The tasting notes made every product feel special.", "Jon F.", "Subscriber")));

    private static PageTemplateDto JewelryTemplate() => Template(
        "jewelry-atelier",
        "Jewelry Atelier",
        "A luxury jewelry homepage with editorial storytelling, collection tiles, trust cues, and inquiry capture.",
        "Jewelry",
        "template-jewelry",
        "#9333ea",
        CommerceLayout(
            brand: "Vera Atelier",
            heroTitle: "Fine jewelry for modern heirlooms",
            heroSubtitle: "Sculptural rings, everyday gold, and made-to-order pieces crafted in small runs.",
            imageUrl: "https://images.unsplash.com/photo-1515562141207-7a88fb7ce338?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Explore collections",
            featureItems: [
                ("Everyday gold", "Minimal pieces with weight, polish, and lasting wearability."),
                ("Occasion pieces", "Gemstone settings designed for celebration and ceremony."),
                ("Custom work", "A guided process for made-to-order rings and keepsakes.")
            ],
            bannerTitle: "Complimentary resizing",
            bannerSubtitle: "Enjoy one free resize on eligible rings within 60 days of purchase.",
            testimonialOne: ("The piece felt personal from packaging to fit.", "Rina A.", "Customer"),
            testimonialTwo: ("The custom process was clear, calm, and beautifully handled.", "Leah M.", "Client")));

    private static PageTemplateDto SportsTemplate() => Template(
        "sports-gear",
        "Sports Gear",
        "An energetic athletic store layout for categories, featured drops, performance claims, and community proof.",
        "Sports",
        "template-sports",
        "#ea580c",
        CommerceLayout(
            brand: "Stride Supply",
            heroTitle: "Performance gear for training days",
            heroSubtitle: "Footwear, apparel, recovery tools, and accessories for runners and hybrid athletes.",
            imageUrl: "https://images.unsplash.com/photo-1517963879433-6ad2b056d712?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Train with better gear",
            featureItems: [
                ("Run essentials", "Shoes, socks, hydration, and reflective accessories."),
                ("Recovery", "Mobility tools and compression gear for the work between sessions."),
                ("Weather-ready", "Layers built for wind, rain, heat, and early mornings.")
            ],
            bannerTitle: "Race season bundle",
            bannerSubtitle: "Save on hydration, socks, and recovery tools when you build a kit.",
            testimonialOne: ("The gear guide helped me prep for my first half marathon.", "Chris M.", "Runner"),
            testimonialTwo: ("Fast shipping before race weekend saved me.", "Taylor G.", "Customer")));

    private static PageTemplateDto DigitalProductsTemplate() => Template(
        "digital-products",
        "Digital Products",
        "A creator commerce page for templates, downloads, bundles, social proof, and email conversion.",
        "Digital Products",
        "template-digital",
        "#4f46e5",
        CommerceLayout(
            brand: "Creator Kits",
            heroTitle: "Digital templates that help creators ship faster",
            heroSubtitle: "Notion dashboards, launch planners, content calendars, and shop-ready digital bundles.",
            imageUrl: "https://images.unsplash.com/photo-1497366754035-f200968a6e72?auto=format&fit=crop&w=1600&q=80",
            featureHeading: "Download-ready systems",
            featureItems: [
                ("Launch planners", "Campaign, offer, and content planning templates for digital sellers."),
                ("Content systems", "Reusable calendars, briefs, and production trackers."),
                ("Business dashboards", "Track revenue, tasks, assets, and customer requests.")
            ],
            bannerTitle: "Bundle and save",
            bannerSubtitle: "Get the full creator operating system with all templates and future updates.",
            testimonialOne: ("The launch kit replaced three separate planning tools.", "Priya N.", "Creator"),
            testimonialTwo: ("I shipped my digital product page in one afternoon.", "Devon L.", "Designer")));

    private static PageTemplateDto BlankTemplate() => Template(
        BlankTemplateId,
        "Blank Commerce Page",
        "Start from an empty canvas and add store header, product grids, categories, and sections manually.",
        "Blank Page",
        "template-blank",
        "#64748b",
        new PageLayoutDocument());

    private static PageLayoutDocument CommerceLayout(
        string brand,
        string heroTitle,
        string heroSubtitle,
        string imageUrl,
        string featureHeading,
        IReadOnlyCollection<(string Title, string Text)> featureItems,
        string bannerTitle,
        string bannerSubtitle,
        (string Quote, string Author, string Role) testimonialOne,
        (string Quote, string Author, string Role) testimonialTwo)
    {
        return Layout(
            S("navBar", Json(new
            {
                brand,
                links = new[]
                {
                    new { text = "Home", url = "/" },
                    new { text = "Shop", url = "/products" },
                    new { text = "Cart", url = "/cart" },
                    new { text = "Contact", url = "#contact" }
                },
                ctaText = "Shop now",
                ctaUrl = "/products"
            }), """{"backgroundColor":"#ffffff","padding":"18px 28px"}"""),
            S("hero", Json(new
            {
                title = heroTitle,
                subtitle = heroSubtitle,
                imageUrl,
                buttonText = "Shop collection",
                buttonUrl = "/products"
            }), """{"minHeight":"620px"}"""),
            S("categoryGrid", """{"heading":"Shop by category","columns":3}""", """{"backgroundColor":"#ffffff","padding":"56px 24px"}"""),
            S("featuredProducts", """{"heading":"Featured products","columns":4,"limit":8,"productIds":[]}""", """{"backgroundColor":"#f8fafc","padding":"64px 24px"}"""),
            S("featureGrid", Json(new
            {
                heading = featureHeading,
                subtitle = "Merchandising blocks you can edit, reorder, or remove in the builder.",
                items = featureItems.Select(item => new { title = item.Title, text = item.Text }).ToArray()
            }), """{"backgroundColor":"#ffffff","padding":"72px 24px"}"""),
            S("banner", Json(new
            {
                title = bannerTitle,
                subtitle = bannerSubtitle,
                buttonText = "Browse products",
                buttonUrl = "/products"
            }), """{"backgroundColor":"#111827","color":"#ffffff","padding":"40px","borderRadius":"8px"}"""),
            S("testimonials", Json(new
            {
                heading = "Customers are already shopping",
                items = new[]
                {
                    new { quote = testimonialOne.Quote, author = testimonialOne.Author, role = testimonialOne.Role },
                    new { quote = testimonialTwo.Quote, author = testimonialTwo.Author, role = testimonialTwo.Role }
                }
            }), """{"backgroundColor":"#f8fafc","padding":"64px 24px"}"""),
            S("form", """{"title":"Get drops, offers, and restock alerts","buttonText":"Join list"}""", """{"backgroundColor":"#ffffff","padding":"72px 24px"}""",
                S("input", """{"label":"Email","placeholder":"you@example.com","inputType":"email","required":true}""")),
            S("footer", Json(new
            {
                brand,
                text = "Secure checkout, fast fulfillment, and customer support for every order.",
                links = new[] { "Shipping", "Returns", "Privacy", "Contact" }
            }), """{"backgroundColor":"#111827","color":"#ffffff","padding":"40px 24px"}"""));
    }

    private static PageTemplateDto Template(
        string id,
        string title,
        string description,
        string category,
        string thumbnailClass,
        string accentColor,
        PageLayoutDocument layout)
    {
        return new PageTemplateDto
        {
            Id = id,
            Title = title,
            Description = description,
            Category = category,
            ThumbnailClass = thumbnailClass,
            AccentColor = accentColor,
            Layout = layout
        };
    }

    private static PageLayoutDocument Layout(params PageSectionDefinition[] sections)
    {
        var layout = new PageLayoutDocument { PageId = "template", Sections = sections.ToList() };
        for (var i = 0; i < layout.Sections.Count; i++)
        {
            layout.Sections[i].Order = i + 1;
        }

        return layout;
    }

    private static PageSectionDefinition S(
        string type,
        string propsJson,
        params PageSectionDefinition[] children)
    {
        return S(type, propsJson, "{}", children);
    }

    private static PageSectionDefinition S(
        string type,
        string propsJson,
        string stylesJson,
        params PageSectionDefinition[] children)
    {
        var section = new PageSectionDefinition
        {
            Id = Guid.NewGuid().ToString("n"),
            Type = type,
            Props = JsonNode.Parse(propsJson)?.AsObject() ?? [],
            Styles = JsonNode.Parse(stylesJson)?.AsObject() ?? [],
            Children = children.ToList()
        };

        for (var i = 0; i < section.Children.Count; i++)
        {
            section.Children[i].Order = i + 1;
        }

        return section;
    }

    private static string Json(object value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static void AssignFreshIds(IEnumerable<PageSectionDefinition> sections)
    {
        var order = 1;
        foreach (var section in sections)
        {
            section.Id = Guid.NewGuid().ToString("n");
            section.Order = order++;
            AssignFreshIds(section.Children);
        }
    }
}
