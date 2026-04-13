namespace FileForgeApi.Features.CsvToJson;

public sealed record CsvToJsonResponse(List<Dictionary<string, string>> Rows);
