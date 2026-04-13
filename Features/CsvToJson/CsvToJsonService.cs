using System.Text.Json;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Json;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;
using MiniExcelLibs.Csv;

namespace FileForgeApi.Features.CsvToJson;

public sealed class CsvToJsonService(ILogger<CsvToJsonService> logger, IDocumentFetchService documentFetchService) : ICsvToJsonService
{
    public async Task<Result<CsvToJsonResponse>> ConvertAsync(CsvToJsonRequest? request)
    {
        var validation = CsvToJsonValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<CsvToJsonResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<CsvToJsonResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        var inferTypes = request!.InferTypes == true;
        var config = BuildCsvConfiguration(request!);

        List<object> queryResult;
        try
        {
            using var stream = new MemoryStream(fileBytes!);
            var asyncRows = await stream.QueryAsync(
                useHeaderRow: true,
                excelType: ExcelType.CSV,
                configuration: config);
            queryResult = asyncRows.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            CsvToJsonServiceLogging.CsvReadFailed(logger, ex);
            return Result<CsvToJsonResponse>.Failure($"No se pudo leer el CSV: {ex.Message}");
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

        return Result<CsvToJsonResponse>.Success(new CsvToJsonResponse(rows));
    }

    private static CsvConfiguration BuildCsvConfiguration(CsvToJsonRequest request)
    {
        var config = new CsvConfiguration();
        if (!string.IsNullOrEmpty(request.Separator))
            config.Seperator = request.Separator[0];
        if (EncodingHelper.TryGetEncoding(request.Encoding, out var encoding) && encoding is not null)
            config.StreamReaderFunc = stream => new StreamReader(stream, encoding);
        return config;
    }
}
