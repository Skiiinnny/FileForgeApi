using FileForgeApi.Shared.Pagination;

namespace FileForgeApi.Features.ExcelToJson;

public sealed record ExcelToJsonRequest(
    string? Base64Content,
    string? DocumentUrl = null,
    bool? InferTypes = false,
    int? Page = null,
    int? PageSize = null) : PaginationParams
{
    public override int? Page { get; init; } = Page;
    public override int? PageSize { get; init; } = PageSize;
}
