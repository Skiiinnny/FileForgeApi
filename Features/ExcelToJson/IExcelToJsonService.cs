using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelToJson;

public interface IExcelToJsonService
{
    Task<Result<ExcelToJsonResponse>> ConvertAsync(ExcelToJsonRequest? request);
}
