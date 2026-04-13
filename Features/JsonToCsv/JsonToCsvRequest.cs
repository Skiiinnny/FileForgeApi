namespace FileForgeApi.Features.JsonToCsv;

public sealed record JsonToCsvRequest(
    List<Dictionary<string, string>> Rows,
    string? Separator = null,
    string? Encoding = null,
    string? NewLine = null);
