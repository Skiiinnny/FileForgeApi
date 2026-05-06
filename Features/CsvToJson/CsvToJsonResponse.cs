using System.Text.Json;
using FileForgeApi.Shared.Pagination;

namespace FileForgeApi.Features.CsvToJson;

public sealed record CsvToJsonResponse(PaginatedResponse<Dictionary<string, JsonElement>> Rows);
