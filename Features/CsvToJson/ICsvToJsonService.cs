using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.CsvToJson;

public interface ICsvToJsonService
{
    Task<Result<CsvToJsonResponse>> ConvertAsync(CsvToJsonRequest? request);
}
