namespace FileForgeApi.Features.ExcelAppendRows;

public sealed record ExcelAppendRowsRequest(
    string? Base64Content,
    List<Dictionary<string, string>> Rows,
    string? DocumentUrl = null);
