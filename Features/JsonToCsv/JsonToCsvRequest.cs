using System.Text.Json;

namespace FileForgeApi.Features.JsonToCsv;

public sealed record JsonToCsvRequest(
    List<Dictionary<string, JsonElement>> Rows,
    string? Separator = null,
    string? Encoding = null,
    string? NewLine = null);
