namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public sealed record CsvToExcelMultiSheetRequest(Dictionary<string, string> Sheets);
