using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToCsv;

public interface IBase64ToCsvService
{
    Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToCsvRequest? request);
}
