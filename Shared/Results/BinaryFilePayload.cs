namespace FileForgeApi.Shared.Results;

public sealed record BinaryFilePayload(byte[] Content, string ContentType, string DownloadFileName);
