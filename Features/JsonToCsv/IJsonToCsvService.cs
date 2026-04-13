using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToCsv;

public interface IJsonToCsvService
{
    Task<Result<JsonToCsvResponse>> ConvertAsync(JsonToCsvRequest? request);
}
