using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.CsvToExcel;

public interface ICsvToExcelService
{
    Task<Result<CsvToExcelResponse>> ConvertAsync(CsvToExcelRequest? request);
}
