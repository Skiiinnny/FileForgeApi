using System.Text.Json;
using FileForgeApi.Shared.Pagination;

namespace FileForgeApi.Features.ExcelToJson;

public sealed record ExcelToJsonResponse(PaginatedResponse<Dictionary<string, JsonElement>> Rows);
