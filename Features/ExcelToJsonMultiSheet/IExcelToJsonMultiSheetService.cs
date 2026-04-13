using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public interface IExcelToJsonMultiSheetService
{
    Task<Result<ExcelToJsonMultiSheetResponse>> ConvertAsync(ExcelToJsonMultiSheetRequest? request);
}
