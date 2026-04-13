namespace FileForgeApi.Features.ExcelMetadata;

public sealed record ExcelMetadataRequest(string? Base64Content, string? DocumentUrl = null);
