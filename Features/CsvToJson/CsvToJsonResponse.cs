using System.Text.Json;

namespace FileForgeApi.Features.CsvToJson;

public sealed record CsvToJsonResponse(List<Dictionary<string, JsonElement>> Rows);
