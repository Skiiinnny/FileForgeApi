namespace FileForgeApi.Features.ExcelFromTemplate;

public sealed record ExcelFromTemplateRequest(
    string? Base64Template,
    List<Dictionary<string, string>> Rows,
    string? TemplateUrl = null);
