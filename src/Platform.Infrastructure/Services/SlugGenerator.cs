using System.Text;
using System.Text.RegularExpressions;

namespace Platform.Infrastructure.Services;

internal static partial class SlugGenerator
{
    public static string Create(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (char.IsWhiteSpace(character) || character is '-' or '_')
            {
                builder.Append('-');
            }
        }

        var slug = DuplicateDashes().Replace(builder.ToString(), "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("n")[..8] : slug;
    }

    [GeneratedRegex("-+")]
    private static partial Regex DuplicateDashes();
}
