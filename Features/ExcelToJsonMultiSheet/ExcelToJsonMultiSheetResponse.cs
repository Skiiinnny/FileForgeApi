using System.Text.Json;
using FileForgeApi.Shared.Pagination;

namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public sealed record ExcelToJsonMultiSheetResponse(Dictionary<string, PaginatedResponse<Dictionary<string, JsonElement>>> Sheets);
