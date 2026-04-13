using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;

namespace FileForgeApi.Features.ExcelFromTemplate;

public sealed class ExcelFromTemplateService(ILogger<ExcelFromTemplateService> logger, IDocumentFetchService documentFetchService) : IExcelFromTemplateService
{
    public async Task<Result<ExcelFromTemplateResponse>> MergeFromTemplateAsync(ExcelFromTemplateRequest? request)
    {
        var validation = ExcelFromTemplateValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<ExcelFromTemplateResponse>.Failure(validation.Error!);

        var (templateBytes, useUrl, newRows) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.TemplateUrl!);
            if (!fetchResult.IsSuccess)
                return Result<ExcelFromTemplateResponse>.Failure(fetchResult.Error!);
            templateBytes = fetchResult.Value;
        }

        try
        {
            var existingRows = await ReadExcelAsRows(templateBytes!);

            var allColumns = GetOrderedColumns(existingRows, newRows);
            var normalizedExisting = NormalizeRows(existingRows, allColumns);
            var normalizedNew = NormalizeRows(newRows, allColumns);
            var mergedRows = normalizedExisting.Concat(normalizedNew).ToList();

            byte[] excelBytes;
            using (var stream = new MemoryStream())
            {
                await stream.SaveAsAsync(mergedRows);
                excelBytes = stream.ToArray();
            }

            var base64 = Convert.ToBase64String(excelBytes);
            return Result<ExcelFromTemplateResponse>.Success(new ExcelFromTemplateResponse(base64));
        }
        catch (Exception ex)
        {
            ExcelFromTemplateServiceLogging.MergeFromTemplateFailed(logger, ex);
            return Result<ExcelFromTemplateResponse>.Failure($"No se pudo fusionar la plantilla con los datos: {ex.Message}");
        }
    }

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

    private static List<string> GetOrderedColumns(
        List<Dictionary<string, string>> existingRows,
        List<Dictionary<string, string>> newRows)
    {
        var columnSet = new HashSet<string>(StringComparer.Ordinal);
        var ordered = new List<string>();

        if (existingRows.Count > 0)
        {
            foreach (var key in existingRows[0].Keys)
            {
                columnSet.Add(key);
                ordered.Add(key);
            }
        }

        foreach (var row in newRows)
        {
            foreach (var key in row.Keys)
            {
                if (columnSet.Add(key))
                    ordered.Add(key);
            }
        }

        return ordered;
    }

    private static List<Dictionary<string, string>> NormalizeRows(
        List<Dictionary<string, string>> rows,
        List<string> columns)
    {
        if (columns.Count == 0)
            return rows;

        var result = new List<Dictionary<string, string>>();
        foreach (var row in rows)
        {
            var dict = new Dictionary<string, string>();
            foreach (var col in columns)
                dict[col] = row.TryGetValue(col, out var val) ? val : string.Empty;
            result.Add(dict);
        }
        return result;
    }
}
