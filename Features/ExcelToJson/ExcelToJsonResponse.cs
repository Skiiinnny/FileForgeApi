using System.Text.Json;

namespace FileForgeApi.Features.ExcelToJson;

public sealed record ExcelToJsonResponse(List<Dictionary<string, JsonElement>> Rows);
