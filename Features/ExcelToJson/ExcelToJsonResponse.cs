namespace FileForgeApi.Features.ExcelToJson;

public sealed record ExcelToJsonResponse(List<Dictionary<string, string>> Rows);
