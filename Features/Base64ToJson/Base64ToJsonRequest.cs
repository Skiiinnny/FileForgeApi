namespace FileForgeApi.Features.Base64ToJson;

public sealed record Base64ToJsonRequest(
    string? Base64Content,
    string? Filename = "file.json",
    string? DocumentUrl = null);
