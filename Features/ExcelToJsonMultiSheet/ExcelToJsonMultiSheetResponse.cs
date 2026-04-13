namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public sealed record ExcelToJsonMultiSheetResponse(Dictionary<string, List<Dictionary<string, string>>> Sheets);
