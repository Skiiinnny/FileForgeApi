namespace FileForgeApi.Shared.Pagination;

public record PaginationParams
{
    public virtual int? Page { get; init; }
    public virtual int? PageSize { get; init; }

    public int ActualPage => Page ?? 1;
    public int ActualPageSize => PageSize ?? 100;

    public int Skip => (ActualPage - 1) * ActualPageSize;
}
