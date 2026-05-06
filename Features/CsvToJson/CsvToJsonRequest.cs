using FileForgeApi.Shared.Pagination;

namespace FileForgeApi.Features.CsvToJson;

public sealed record CsvToJsonRequest(
    string? Base64Content,
    string? Separator = null,
    string? Encoding = null,
    string? DocumentUrl = null,
    bool? InferTypes = false,
    int? Page = null,
    int? PageSize = null) : PaginationParams
{
    public override int? Page { get; init; } = Page;
    public override int? PageSize { get; init; } = PageSize;
}
