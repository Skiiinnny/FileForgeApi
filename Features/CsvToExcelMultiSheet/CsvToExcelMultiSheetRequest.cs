namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public sealed record CsvToExcelMultiSheetRequest(
    Dictionary<string, string> Sheets,
    string? Separator = null,
    string? Encoding = null);
