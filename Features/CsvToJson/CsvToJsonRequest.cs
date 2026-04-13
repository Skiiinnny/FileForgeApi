namespace FileForgeApi.Features.CsvToJson;

public sealed record CsvToJsonRequest(
    string? Base64Content,
    string? Separator = null,
    string? Encoding = null,
    string? DocumentUrl = null);
