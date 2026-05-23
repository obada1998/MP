using Platform.Application.Rendering;

namespace Platform.Tests;

public sealed class PageLayoutValidatorTests
{
    [Fact]
    public void Validator_accepts_known_components_and_normalizes_order()
    {
        var validator = new PageLayoutValidator();

        var result = validator.Validate("""
        {"pageId":"home","sections":[{"type":"text","order":2,"props":{"text":"Body"}},{"type":"hero","order":1,"props":{"title":"Welcome"}}]}
        """);

        Assert.True(result.IsValid, string.Join("; ", result.Errors));
        Assert.Equal("hero", result.Layout!.Sections[0].Type);
    }

    [Fact]
    public void Validator_rejects_unknown_and_unsafe_components()
    {
        var validator = new PageLayoutValidator();

        var result = validator.Validate("""
        {"pageId":"home","sections":[{"type":"unknown","order":1,"props":{}},{"type":"customHtml","order":2,"props":{"html":"<script>alert(1)</script>"}}]}
        """);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("Unknown page component type"));
        Assert.Contains(result.Errors, error => error.Contains("Custom HTML"));
    }

    [Fact]
    public void Validator_accepts_nested_editor_elements()
    {
        var validator = new PageLayoutValidator();

        var result = validator.Validate("""
        {"pageId":"home","sections":[{"type":"container","order":1,"props":{},"children":[{"type":"text","order":2,"props":{"text":"Body"}},{"type":"button","order":1,"props":{"text":"Shop"}}]}]}
        """);

        Assert.True(result.IsValid, string.Join("; ", result.Errors));
        var container = result.Layout!.Sections[0];
        Assert.False(string.IsNullOrWhiteSpace(container.Id));
        Assert.Equal("button", container.Children[0].Type);
        Assert.Equal(1, container.Children[0].Order);
    }
}
