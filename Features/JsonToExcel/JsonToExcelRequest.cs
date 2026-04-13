using System.Text.Json;

namespace FileForgeApi.Features.JsonToExcel;

public sealed record JsonToExcelRequest(List<Dictionary<string, JsonElement>> Rows);
