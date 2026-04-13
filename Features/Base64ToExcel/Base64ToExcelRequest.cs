namespace FileForgeApi.Features.Base64ToExcel;

public sealed record Base64ToExcelRequest(
    string? Base64Content,
    string? Filename = "file.xlsx",
    string? DocumentUrl = null);
