namespace FileForgeApi.Features.CsvToExcel;

public sealed record CsvToExcelRequest(
    string? Base64Content,
    string? Separator = null,
    string? Encoding = null,
    string? DocumentUrl = null);
