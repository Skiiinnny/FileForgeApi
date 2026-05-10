using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToJson;

public sealed class Base64ToJsonService(ILogger<Base64ToJsonService> logger, IDocumentFetchService documentFetchService) : IBase64ToJsonService
{
    public async Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToJsonRequest? request)
    {
        Base64ToJsonServiceLogging.ConvertRequested(logger, request?.Filename);

        var validation = Base64ToJsonValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            Base64ToJsonServiceLogging.ValidationFailed(logger, validation.Error!);
            return Result<BinaryFilePayload>.Failure(validation.Error!);
        }

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
            {
                Base64ToJsonServiceLogging.ValidationFailed(logger, fetchResult.Error!);
                return Result<BinaryFilePayload>.Failure(fetchResult.Error!);
            }
            fileBytes = fetchResult.Value;
        }

        string filename = request!.Filename ?? "file.json";

        Base64ToJsonServiceLogging.ConvertSucceeded(logger, fileBytes!.Length);
        return Result<BinaryFilePayload>.Success(
            new BinaryFilePayload(fileBytes!, "application/json", filename));
    }
}
