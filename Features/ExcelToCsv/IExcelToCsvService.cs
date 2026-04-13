using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelToCsv;

public interface IExcelToCsvService
{
    Task<Result<ExcelToCsvResponse>> ConvertAsync(ExcelToCsvRequest? request);
}
