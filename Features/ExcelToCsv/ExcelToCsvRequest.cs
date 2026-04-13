namespace FileForgeApi.Features.ExcelToCsv;

public sealed record ExcelToCsvRequest(
    string? Base64Content,
    string? Separator = null,
    string? Encoding = null,
    string? NewLine = null,
    string? DocumentUrl = null);
