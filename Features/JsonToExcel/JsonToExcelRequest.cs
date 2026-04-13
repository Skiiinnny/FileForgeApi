namespace FileForgeApi.Features.JsonToExcel;

public sealed record JsonToExcelRequest(List<Dictionary<string, string>> Rows);
