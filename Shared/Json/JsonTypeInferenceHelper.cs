using System.Globalization;
using System.Text.Json;

namespace FileForgeApi.Shared.Json;

public static class JsonTypeInferenceHelper
{
    public static JsonElement TryInfer(string raw)
    {
        if (raw is null || raw.Equals("null", StringComparison.OrdinalIgnoreCase))
            return JsonDocument.Parse("null").RootElement.Clone();

        if (bool.TryParse(raw, out var boolVal))
            return JsonDocument.Parse(boolVal ? "true" : "false").RootElement.Clone();

        if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
            return JsonDocument.Parse(longVal.ToString(CultureInfo.InvariantCulture)).RootElement.Clone();

        if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var doubleVal))
            return JsonDocument.Parse(doubleVal.ToString(CultureInfo.InvariantCulture)).RootElement.Clone();

        var escaped = JsonEncodedText.Encode(raw).ToString();
        return JsonDocument.Parse($"\"{escaped}\"").RootElement.Clone();
    }

    public static JsonElement WrapString(string? raw)
    {
        if (raw is null)
            return JsonDocument.Parse("null").RootElement.Clone();

        var escaped = JsonEncodedText.Encode(raw).ToString();
        return JsonDocument.Parse($"\"{escaped}\"").RootElement.Clone();
    }
}
