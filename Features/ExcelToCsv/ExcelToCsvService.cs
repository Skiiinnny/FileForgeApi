using System.Text;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;
using MiniExcelLibs.Csv;

namespace FileForgeApi.Features.ExcelToCsv;

public sealed class ExcelToCsvService(ILogger<ExcelToCsvService> logger, IDocumentFetchService documentFetchService) : IExcelToCsvService
{
    public async Task<Result<ExcelToCsvResponse>> ConvertAsync(ExcelToCsvRequest? request)
    {
        var validation = ExcelToCsvValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<ExcelToCsvResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<ExcelToCsvResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        bool hasCustomOptions = HasCustomOptions(request!);

        byte[] csvBytes;
        try
        {
            if (!hasCustomOptions)
            {
                using var excelStream = new MemoryStream(fileBytes!);
                using var csvStream = new MemoryStream();
                MiniExcel.ConvertXlsxToCsv(excelStream, csvStream);
                csvBytes = csvStream.ToArray();
            }
            else
            {
                var rows = await ReadExcelAsRows(fileBytes!);
                using var csvStream = new MemoryStream();
                var config = BuildCsvConfiguration(request!);
                await csvStream.SaveAsAsync(rows, excelType: ExcelType.CSV, configuration: config);
                csvBytes = csvStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            ExcelToCsvServiceLogging.ExcelToCsvConversionFailed(logger, ex);
            return Result<ExcelToCsvResponse>.Failure($"No se pudo convertir Excel a CSV: {ex.Message}");
        }

        var base64 = Convert.ToBase64String(csvBytes);
        return Result<ExcelToCsvResponse>.Success(new ExcelToCsvResponse(base64));
    }

    private static bool HasCustomOptions(ExcelToCsvRequest request) =>
        !string.IsNullOrEmpty(request.Separator) ||
        !string.IsNullOrEmpty(request.Encoding) ||
        !string.IsNullOrEmpty(request.NewLine);

    private static async Task<List<Dictionary<string, string>>> ReadExcelAsRows(byte[] fileBytes)
    {
        using var stream = new MemoryStream(fileBytes);
        var asyncRows = await stream.QueryAsync(useHeaderRow: true);
        var queryResult = asyncRows.Cast<object>().ToList();

        var rows = new List<Dictionary<string, string>>();
        foreach (var row in queryResult)
        {
            if (row is not IDictionary<string, object> dictionaryRow)
                continue;

            var dict = new Dictionary<string, string>();
            foreach (var kvp in dictionaryRow)
            {
                var key = kvp.Key?.ToString() ?? "ColumnaSinNombre";
                var value = kvp.Value?.ToString() ?? string.Empty;
                dict[key] = value;
            }
            rows.Add(dict);
        }
        return rows;
    }

    private static CsvConfiguration BuildCsvConfiguration(ExcelToCsvRequest request)
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
