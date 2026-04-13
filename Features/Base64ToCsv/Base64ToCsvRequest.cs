namespace FileForgeApi.Features.Base64ToCsv;

public sealed record Base64ToCsvRequest(
    string? Base64Content,
    string? Filename = "file.csv",
    string? DocumentUrl = null);
