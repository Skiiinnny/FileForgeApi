using FileForgeApi.Shared.Documents;

namespace FileForgeApi.Features.Base64ToJson;

public sealed class Base64ToJsonService(ILogger<Base64ToJsonService> logger, IDocumentFetchService documentFetchService) : IBase64ToJsonService
{
    public async Task<IResult> ConvertAsync(Base64ToJsonRequest? request)
    {
        Base64ToJsonServiceLogging.ConvertRequested(logger, request?.Filename);

        var validation = Base64ToJsonValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            Base64ToJsonServiceLogging.ValidationFailed(logger, validation.Error!);
            return Results.BadRequest(new { error = validation.Error });
        }

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
            {
                Base64ToJsonServiceLogging.ValidationFailed(logger, fetchResult.Error!);
                return Results.BadRequest(new { error = fetchResult.Error });
            }
            fileBytes = fetchResult.Value;
        }

        string filename = request!.Filename ?? "file.json";

        IResult result = Results.File(fileBytes!, "application/json", filename);
        Base64ToJsonServiceLogging.ConvertSucceeded(logger, fileBytes!.Length);
        return result;
    }
}
