using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;
using MiniExcelLibs.Csv;

namespace FileForgeApi.Features.JsonToCsv;

public sealed class JsonToCsvService(ILogger<JsonToCsvService> logger) : IJsonToCsvService
{
    public async Task<Result<JsonToCsvResponse>> ConvertAsync(JsonToCsvRequest? request)
    {
        var validation = JsonToCsvValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<JsonToCsvResponse>.Failure(validation.Error!);

        var rows = validation.Value!;
        var config = BuildCsvConfiguration(request!);

        byte[] csvBytes;
        try
        {
            using var stream = new MemoryStream();
            await stream.SaveAsAsync(rows, excelType: ExcelType.CSV, configuration: config);
            csvBytes = stream.ToArray();
        }
        catch (Exception ex)
        {
            JsonToCsvServiceLogging.CsvGenerationFailed(logger, ex);
            return Result<JsonToCsvResponse>.Failure($"No se pudo generar el CSV: {ex.Message}");
        }

        var base64 = Convert.ToBase64String(csvBytes);
        return Result<JsonToCsvResponse>.Success(new JsonToCsvResponse(base64));
    }

    private static CsvConfiguration BuildCsvConfiguration(JsonToCsvRequest request)
    {
        var config = new CsvConfiguration();
        if (!string.IsNullOrEmpty(request.Separator))
            config.Seperator = request.Separator[0];
        if (request.NewLine == "\n")
            config.NewLine = "\n";
        if (EncodingHelper.TryGetEncoding(request.Encoding, out var encoding) && encoding is not null)
            config.StreamWriterFunc = stream => new StreamWriter(stream, encoding);
        return config;
    }
}
