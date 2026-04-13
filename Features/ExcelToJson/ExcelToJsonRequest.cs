namespace FileForgeApi.Features.ExcelToJson;

public sealed record ExcelToJsonRequest(string? Base64Content, string? DocumentUrl = null);
