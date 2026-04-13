namespace FileForgeApi.Features.ExcelMetadata;

public sealed record ExcelMetadataResponse(
    string? Creator = null,
    string? Created = null,
    string? Modified = null,
    string? LastModifiedBy = null,
    string? Title = null,
    string? Subject = null,
    string? Keywords = null,
    string? Description = null);
