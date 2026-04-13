using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToExcel;

public interface IJsonToExcelService
{
    Task<Result<JsonToExcelResponse>> ConvertAsync(JsonToExcelRequest? request);
}
