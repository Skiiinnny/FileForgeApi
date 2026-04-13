using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public interface IJsonToExcelMultiSheetService
{
    Task<Result<JsonToExcelMultiSheetResponse>> ConvertAsync(JsonToExcelMultiSheetRequest? request);
}
