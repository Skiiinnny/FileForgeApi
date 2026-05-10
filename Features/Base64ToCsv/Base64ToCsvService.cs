using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToCsv;

public sealed class Base64ToCsvService(ILogger<Base64ToCsvService> logger, IDocumentFetchService documentFetchService) : IBase64ToCsvService
{
    public async Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToCsvRequest? request)
    {
        Base64ToCsvServiceLogging.ConvertRequested(logger, request?.Filename);

        var validation = Base64ToCsvValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            Base64ToCsvServiceLogging.ValidationFailed(logger, validation.Error!);
            return Result<BinaryFilePayload>.Failure(validation.Error!);
        }

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
            {
                Base64ToCsvServiceLogging.ValidationFailed(logger, fetchResult.Error!);
                return Result<BinaryFilePayload>.Failure(fetchResult.Error!);
            }
            fileBytes = fetchResult.Value;
        }

        string filename = request!.Filename ?? "file.csv";

        Base64ToCsvServiceLogging.ConvertSucceeded(logger, fileBytes!.Length);
        return Result<BinaryFilePayload>.Success(
            new BinaryFilePayload(fileBytes!, "text/csv", filename));
    }
}
