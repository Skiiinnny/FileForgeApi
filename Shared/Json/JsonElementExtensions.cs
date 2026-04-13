using System.Text.Json;

namespace FileForgeApi.Shared.Json;

public static class JsonElementExtensions
{
    public static object? ToClrObject(this JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => el.ToString()
    };
}
