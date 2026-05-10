using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToJson;

public interface IBase64ToJsonService
{
    Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToJsonRequest? request);
}
