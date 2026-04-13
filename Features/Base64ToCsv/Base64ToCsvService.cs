using FileForgeApi.Shared.Documents;

namespace FileForgeApi.Features.Base64ToCsv;

public sealed class Base64ToCsvService(ILogger<Base64ToCsvService> logger, IDocumentFetchService documentFetchService) : IBase64ToCsvService
{
    public async Task<IResult> ConvertAsync(Base64ToCsvRequest? request)
    {
        Base64ToCsvServiceLogging.ConvertRequested(logger, request?.Filename);

        var validation = Base64ToCsvValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            Base64ToCsvServiceLogging.ValidationFailed(logger, validation.Error!);
            return Results.BadRequest(new { error = validation.Error });
        }

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
            {
                Base64ToCsvServiceLogging.ValidationFailed(logger, fetchResult.Error!);
                return Results.BadRequest(new { error = fetchResult.Error });
            }
            fileBytes = fetchResult.Value;
        }

        string filename = request!.Filename ?? "file.csv";

        IResult result = Results.File(fileBytes!, "text/csv", filename);
        Base64ToCsvServiceLogging.ConvertSucceeded(logger, fileBytes!.Length);
        return result;
    }
}
