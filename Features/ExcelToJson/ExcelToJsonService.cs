using System.Text.Json;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Json;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;

namespace FileForgeApi.Features.ExcelToJson;

public sealed class ExcelToJsonService(ILogger<ExcelToJsonService> logger, IDocumentFetchService documentFetchService) : IExcelToJsonService
{
    public async Task<Result<ExcelToJsonResponse>> ConvertAsync(ExcelToJsonRequest? request)
    {
        var validation = ExcelToJsonValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<ExcelToJsonResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<ExcelToJsonResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        var inferTypes = request!.InferTypes == true;

        List<object> queryResult;
        try
        {
            using var stream = new MemoryStream(fileBytes!);
            var asyncRows = await stream.QueryAsync(useHeaderRow: true);
            queryResult = asyncRows.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            ExcelToJsonServiceLogging.ExcelReadFailed(logger, ex);
            return Result<ExcelToJsonResponse>.Failure($"No se pudo leer el Excel: {ex.Message}");
        }

        var rows = new List<Dictionary<string, JsonElement>>();

        foreach (var row in queryResult)
        {
            if (row is not IDictionary<string, object> dictionaryRow)
                continue;

            var dict = new Dictionary<string, JsonElement>();
            foreach (var kvp in dictionaryRow)
            {
                var key = kvp.Key?.ToString() ?? "ColumnaSinNombre";
                var rawValue = kvp.Value?.ToString() ?? string.Empty;
                dict[key] = inferTypes
                    ? JsonTypeInferenceHelper.TryInfer(rawValue)
                    : JsonTypeInferenceHelper.WrapString(rawValue);
            }

            rows.Add(dict);
        }

        return Result<ExcelToJsonResponse>.Success(new ExcelToJsonResponse(rows));
    }
}
