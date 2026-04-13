namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public sealed record ExcelToJsonMultiSheetRequest(string? Base64Content, string? DocumentUrl = null, bool? InferTypes = false);
