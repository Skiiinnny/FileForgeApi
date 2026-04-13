using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public interface ICsvToExcelMultiSheetService
{
    Task<Result<CsvToExcelMultiSheetResponse>> ConvertAsync(CsvToExcelMultiSheetRequest? request);
}
