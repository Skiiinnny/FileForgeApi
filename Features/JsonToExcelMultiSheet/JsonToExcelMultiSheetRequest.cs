namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public sealed record JsonToExcelMultiSheetRequest(Dictionary<string, List<Dictionary<string, string>>> Sheets);
