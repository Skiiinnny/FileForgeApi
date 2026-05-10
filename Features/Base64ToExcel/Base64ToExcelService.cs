using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToExcel;

public sealed class Base64ToExcelService(ILogger<Base64ToExcelService> logger, IDocumentFetchService documentFetchService) : IBase64ToExcelService
{
    public async Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToExcelRequest? request)
    {
        Base64ToExcelServiceLogging.ConvertRequested(logger, request?.Filename);

        var validation = Base64ToExcelValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            Base64ToExcelServiceLogging.ValidationFailed(logger, validation.Error!);
            return Result<BinaryFilePayload>.Failure(validation.Error!);
        }

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
            {
                Base64ToExcelServiceLogging.ValidationFailed(logger, fetchResult.Error!);
                return Result<BinaryFilePayload>.Failure(fetchResult.Error!);
            }
            fileBytes = fetchResult.Value;
        }

        string filename = request!.Filename ?? "file.xlsx";

        Base64ToExcelServiceLogging.ConvertSucceeded(logger, fileBytes!.Length);
        return Result<BinaryFilePayload>.Success(
            new BinaryFilePayload(
                fileBytes!,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename));
    }
}
