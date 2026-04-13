using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelAppendRows;

public interface IExcelAppendRowsService
{
    Task<Result<ExcelAppendRowsResponse>> AppendRowsAsync(ExcelAppendRowsRequest? request);
}
