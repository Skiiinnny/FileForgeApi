using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;
using MiniExcelLibs.Csv;

namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public sealed class CsvToExcelMultiSheetService(ILogger<CsvToExcelMultiSheetService> logger)
    : ICsvToExcelMultiSheetService
{
    public async Task<Result<CsvToExcelMultiSheetResponse>> ConvertAsync(CsvToExcelMultiSheetRequest? request)
    {
        var validation = CsvToExcelMultiSheetValidator.Validate(request);
        var separator = request?.Separator;
        var encoding = request?.Encoding;
        if (!validation.IsSuccess)
            return Result<CsvToExcelMultiSheetResponse>.Failure(validation.Error!);

        var sheetsData = new Dictionary<string, object>();

        foreach (var (sheetName, csvBase64) in request!.Sheets)
        {
            byte[] csvBytes;
            try
            {
                csvBytes = Convert.FromBase64String(csvBase64.Trim());
            }
            catch (FormatException)
            {
                return Result<CsvToExcelMultiSheetResponse>.Failure(
                    $"El contenido de la hoja '{sheetName}' no es una cadena Base64 válida.");
            }

            try
            {
                var rows = await ReadCsvAsRows(csvBytes, separator, encoding);
                sheetsData[sheetName] = rows;
            }
            catch (Exception ex)
            {
                CsvToExcelMultiSheetServiceLogging.CsvParsingFailed(logger, sheetName, ex);
                return Result<CsvToExcelMultiSheetResponse>.Failure(
                    $"No se pudo parsear el CSV de la hoja '{sheetName}': {ex.Message}");
            }
        }

        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        try
        {
            MiniExcel.SaveAs(tempPath, sheetsData);
            var excelBytes = await File.ReadAllBytesAsync(tempPath);
            var base64 = Convert.ToBase64String(excelBytes);
            return Result<CsvToExcelMultiSheetResponse>.Success(new CsvToExcelMultiSheetResponse(base64));
        }
        catch (Exception ex)
        {
            CsvToExcelMultiSheetServiceLogging.ExcelGenerationFailed(logger, ex);
            return Result<CsvToExcelMultiSheetResponse>.Failure($"No se pudo generar el Excel: {ex.Message}");
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static async Task<List<Dictionary<string, string>>> ReadCsvAsRows(
        byte[] csvBytes,
        string? separator,
        string? encoding)
    {
        var config = BuildCsvConfiguration(separator, encoding);
        using var stream = new MemoryStream(csvBytes);
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

    private static CsvConfiguration BuildCsvConfiguration(string? separator, string? encoding)
    {
        var config = new CsvConfiguration();
        if (!string.IsNullOrEmpty(separator))
            config.Seperator = separator[0];
        if (EncodingHelper.TryGetEncoding(encoding, out var enc) && enc is not null)
            config.StreamReaderFunc = stream => new StreamReader(stream, enc);
        return config;
    }
}
