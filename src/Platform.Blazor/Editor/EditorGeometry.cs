using System.Globalization;
using System.Text.Json.Nodes;
using Platform.Application.Pages;

namespace Platform.Blazor.Editor;

public static class EditorGeometry
{
    public const double DefaultCanvasHeight = 1040;
    public const double DefaultCanvasWidth = 1320;
    public const double MinWidth = 72;
    public const double MinHeight = 44;
    public const int DefaultSnapDistance = 8;

    public static readonly IReadOnlyCollection<string> ResizeHandles =
    [
        "nw",
        "n",
        "ne",
        "e",
        "se",
        "s",
        "sw",
        "w"
    ];

    public static EditorElementBox GetBox(PageSectionDefinition section, int index, int depth = 0)
    {
        var fallbackWidth = DefaultWidth(section.Type, depth);
        var fallbackHeight = DefaultHeight(section.Type);
        var width = GetPixel(section.Styles, "width", fallbackWidth);
        var height = GetPixel(section.Styles, "height", GetPixel(section.Styles, "minHeight", fallbackHeight));
        var x = GetPixel(section.Styles, "left", GetPixel(section.Styles, "x", 24 + depth * 16));
        var y = GetPixel(section.Styles, "top", GetPixel(section.Styles, "y", 24 + index * (fallbackHeight + 18)));
        var zIndex = GetInt(section.Styles, "zIndex", section.Order > 0 ? section.Order : index + 1);
        var isVisible = !GetString(section.Styles, "visibility").Equals("hidden", StringComparison.OrdinalIgnoreCase) &&
            !GetString(section.Styles, "display").Equals("none", StringComparison.OrdinalIgnoreCase);
        var isLocked = GetBool(section.Styles, "locked");

        return new EditorElementBox(
            section.Id,
            Math.Max(0, x),
            Math.Max(0, y),
            Math.Max(MinWidth, width),
            Math.Max(MinHeight, height),
            zIndex,
            isVisible,
            isLocked);
    }

    public static void ApplyBox(PageSectionDefinition section, EditorElementBox box)
    {
        section.Styles["position"] = "absolute";
        section.Styles["left"] = FormatPixel(box.X);
        section.Styles["top"] = FormatPixel(box.Y);
        section.Styles["width"] = FormatPixel(box.Width);
        section.Styles["height"] = FormatPixel(box.Height);
        section.Styles["zIndex"] = box.ZIndex.ToString(CultureInfo.InvariantCulture);

        if (box.IsVisible)
        {
            section.Styles.Remove("visibility");
        }
        else
        {
            section.Styles["visibility"] = "hidden";
        }

        section.Styles["locked"] = box.IsLocked ? "true" : "false";
    }

    public static EditorElementBox ClampToCanvas(EditorElementBox box, double canvasWidth, double canvasHeight)
    {
        canvasWidth = Math.Max(MinWidth, canvasWidth);
        canvasHeight = Math.Max(MinHeight, canvasHeight);
        var width = Math.Clamp(box.Width, MinWidth, canvasWidth);
        var height = Math.Clamp(box.Height, MinHeight, canvasHeight);
        var x = Math.Clamp(box.X, 0, Math.Max(0, canvasWidth - width));
        var y = Math.Clamp(box.Y, 0, Math.Max(0, canvasHeight - height));

        return box with { X = x, Y = y, Width = width, Height = height };
    }

    public static EditorSnapResult Snap(
        EditorElementBox proposed,
        IEnumerable<EditorElementBox> boxes,
        double canvasWidth,
        double canvasHeight,
        int snapDistance,
        string? resizeHandle = null)
    {
        var targetsX = BuildTargets(canvasWidth, boxes.SelectMany(box => new[] { box.X, box.X + box.Width / 2, box.X + box.Width }));
        var targetsY = BuildTargets(canvasHeight, boxes.SelectMany(box => new[] { box.Y, box.Y + box.Height / 2, box.Y + box.Height }));
        var snapped = proposed;
        var guides = new List<EditorGuide>();
        var distance = Math.Clamp(snapDistance, 0, 32);

        if (distance <= 0)
        {
            return new EditorSnapResult(ClampToCanvas(snapped, canvasWidth, canvasHeight), guides);
        }

        if (string.IsNullOrWhiteSpace(resizeHandle))
        {
            if (TryFindSnap(targetsX, distance, snapped.X, snapped.X + snapped.Width / 2, snapped.X + snapped.Width) is { } xSnap)
            {
                snapped = snapped with { X = snapped.X + xSnap.Delta };
                guides.Add(new EditorGuide("x", xSnap.Target));
            }

            if (TryFindSnap(targetsY, distance, snapped.Y, snapped.Y + snapped.Height / 2, snapped.Y + snapped.Height) is { } ySnap)
            {
                snapped = snapped with { Y = snapped.Y + ySnap.Delta };
                guides.Add(new EditorGuide("y", ySnap.Target));
            }
        }
        else
        {
            snapped = SnapResizeAxis(snapped, targetsX, distance, resizeHandle, "w", "e", true, guides);
            snapped = SnapResizeAxis(snapped, targetsY, distance, resizeHandle, "n", "s", false, guides);
        }

        return new EditorSnapResult(ClampToCanvas(snapped, canvasWidth, canvasHeight), guides);
    }

    public static string FormatPixel(double value)
    {
        return $"{Math.Round(value, 1).ToString("0.#", CultureInfo.InvariantCulture)}px";
    }

    private static EditorElementBox SnapResizeAxis(
        EditorElementBox box,
        IReadOnlyCollection<double> targets,
        double snapDistance,
        string handle,
        string startHandle,
        string endHandle,
        bool horizontal,
        List<EditorGuide> guides)
    {
        if (handle.Contains(endHandle, StringComparison.OrdinalIgnoreCase))
        {
            var edge = horizontal ? box.X + box.Width : box.Y + box.Height;
            if (TryFindSnap(targets, snapDistance, edge) is { } snap)
            {
                var size = horizontal ? box.Width + snap.Delta : box.Height + snap.Delta;
                if (size >= (horizontal ? MinWidth : MinHeight))
                {
                    box = horizontal ? box with { Width = size } : box with { Height = size };
                    guides.Add(new EditorGuide(horizontal ? "x" : "y", snap.Target));
                }
            }
        }

        if (handle.Contains(startHandle, StringComparison.OrdinalIgnoreCase))
        {
            var edge = horizontal ? box.X : box.Y;
            if (TryFindSnap(targets, snapDistance, edge) is { } snap)
            {
                var size = horizontal ? box.Width - snap.Delta : box.Height - snap.Delta;
                if (size >= (horizontal ? MinWidth : MinHeight))
                {
                    box = horizontal
                        ? box with { X = box.X + snap.Delta, Width = size }
                        : box with { Y = box.Y + snap.Delta, Height = size };
                    guides.Add(new EditorGuide(horizontal ? "x" : "y", snap.Target));
                }
            }
        }

        return box;
    }

    private static IReadOnlyCollection<double> BuildTargets(double canvasSize, IEnumerable<double> elementTargets)
    {
        return new[] { 0, canvasSize / 2, canvasSize }
            .Concat(elementTargets.Where(value => value >= 0 && value <= canvasSize))
            .Distinct()
            .ToArray();
    }

    private static AxisSnap? TryFindSnap(IReadOnlyCollection<double> targets, double snapDistance, params double[] sources)
    {
        AxisSnap? best = null;
        foreach (var source in sources)
        {
            foreach (var target in targets)
            {
                var delta = target - source;
                var distance = Math.Abs(delta);
                if (distance > snapDistance)
                {
                    continue;
                }

                if (best is null || distance < best.Distance)
                {
                    best = new AxisSnap(delta, target, distance);
                }
            }
        }

        return best;
    }

    private static double DefaultWidth(string type, int depth)
    {
        var width = type switch
        {
            "button" => 180,
            "input" => 280,
            "spacer" => 420,
            "text" => 520,
            "image" => 640,
            "logo" => 220,
            "navBar" or "hero" or "footer" => 780,
            _ => 620
        };

        return Math.Max(MinWidth, width - depth * 64);
    }

    private static double DefaultHeight(string type)
    {
        return type switch
        {
            "navBar" => 82,
            "button" => 72,
            "input" => 92,
            "spacer" => 64,
            "text" => 150,
            "image" => 360,
            "logo" => 110,
            "hero" => 240,
            "footer" => 118,
            "columns" or "container" or "form" => 260,
            _ => 180
        };
    }

    private static double GetPixel(JsonObject styles, string key, double fallback)
    {
        var raw = GetString(styles, key);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        raw = raw.Trim();
        if (raw.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            raw = raw[..^2];
        }

        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }

    private static int GetInt(JsonObject styles, string key, int fallback)
    {
        var raw = GetString(styles, key);
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;
    }

    private static bool GetBool(JsonObject styles, string key)
    {
        var raw = GetString(styles, key);
        return bool.TryParse(raw, out var parsed) && parsed;
    }

    private static string GetString(JsonObject styles, string key)
    {
        return styles.TryGetPropertyValue(key, out var node) && node is JsonValue value
            ? value.ToString()
            : string.Empty;
    }

    private sealed record AxisSnap(double Delta, double Target, double Distance);
}
