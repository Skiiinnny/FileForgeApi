namespace FileForgeApi.Shared.Pagination;

public sealed record PaginatedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public static PaginatedResponse<T> Create(IEnumerable<T> items, int totalCount, PaginationParams paginationParams)
    {
        return new PaginatedResponse<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = paginationParams.ActualPage,
            PageSize = paginationParams.ActualPageSize
        };
    }
}
