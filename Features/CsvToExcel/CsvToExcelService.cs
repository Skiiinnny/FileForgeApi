using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;
using MiniExcelLibs.Csv;

namespace FileForgeApi.Features.CsvToExcel;

public sealed class CsvToExcelService(ILogger<CsvToExcelService> logger, IDocumentFetchService documentFetchService) : ICsvToExcelService
{
    public async Task<Result<CsvToExcelResponse>> ConvertAsync(CsvToExcelRequest? request)
    {
        var validation = CsvToExcelValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<CsvToExcelResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<CsvToExcelResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        bool hasCustomOptions = HasCustomOptions(request!);

        byte[] excelBytes;
        try
        {
            if (!hasCustomOptions)
            {
                using var csvStream = new MemoryStream(fileBytes!);
                using var excelStream = new MemoryStream();
                MiniExcel.ConvertCsvToXlsx(csvStream, excelStream);
                excelBytes = excelStream.ToArray();
            }
            else
            {
                var rows = await ReadCsvAsRows(fileBytes!, request!);
                using var excelStream = new MemoryStream();
                await excelStream.SaveAsAsync(rows);
                excelBytes = excelStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            CsvToExcelServiceLogging.CsvToExcelConversionFailed(logger, ex);
            return Result<CsvToExcelResponse>.Failure($"No se pudo convertir CSV a Excel: {ex.Message}");
        }

        var base64 = Convert.ToBase64String(excelBytes);
        return Result<CsvToExcelResponse>.Success(new CsvToExcelResponse(base64));
    }

    private static bool HasCustomOptions(CsvToExcelRequest request) =>
        !string.IsNullOrEmpty(request.Separator) ||
        !string.IsNullOrEmpty(request.Encoding);

    private static async Task<List<Dictionary<string, string>>> ReadCsvAsRows(byte[] fileBytes, CsvToExcelRequest request)
    {
        var config = BuildCsvConfiguration(request);
        using var stream = new MemoryStream(fileBytes);
        var asyncRows = await stream.QueryAsync(useHeaderRow: true, excelType: ExcelType.CSV, configuration: config);
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

    private static CsvConfiguration BuildCsvConfiguration(CsvToExcelRequest request)
    {
        var config = new CsvConfiguration();
        if (!string.IsNullOrEmpty(request.Separator))
            config.Seperator = request.Separator[0];
        if (EncodingHelper.TryGetEncoding(request.Encoding, out var encoding) && encoding is not null)
            config.StreamReaderFunc = stream => new StreamReader(stream, encoding);
        return config;
    }
}
