using System.Text.Json;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Json;
using FileForgeApi.Shared.Results;
using FileForgeApi.Shared.Pagination;
using MiniExcelLibs;

namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public sealed class ExcelToJsonMultiSheetService(ILogger<ExcelToJsonMultiSheetService> logger, IDocumentFetchService documentFetchService)
    : IExcelToJsonMultiSheetService
{
    public async Task<Result<ExcelToJsonMultiSheetResponse>> ConvertAsync(ExcelToJsonMultiSheetRequest? request)
    {
        var validation = ExcelToJsonMultiSheetValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<ExcelToJsonMultiSheetResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<ExcelToJsonMultiSheetResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        var inferTypes = request!.InferTypes == true;
        var sheets = new Dictionary<string, PaginatedResponse<Dictionary<string, JsonElement>>>();

        try
        {
            IEnumerable<string> sheetNames;
            using (var nameStream = new MemoryStream(fileBytes!))
            {
                sheetNames = nameStream.GetSheetNames().ToList();
            }

            foreach (var sheetName in sheetNames)
            {
                using var stream = new MemoryStream(fileBytes!);
                var asyncRows = await stream.QueryAsync(useHeaderRow: true, sheetName: sheetName);
                var queryResult = asyncRows.Cast<object>().ToList();

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

                var paginatedItems = rows
                    .Skip(request!.Skip)
                    .Take(request.ActualPageSize)
                    .ToList();

                var paginatedResponse = PaginatedResponse<Dictionary<string, JsonElement>>.Create(
                    paginatedItems,
                    rows.Count,
                    request);

                sheets[sheetName] = paginatedResponse;
            }
        }
        catch (Exception ex)
        {
            ExcelToJsonMultiSheetServiceLogging.ExcelMultiSheetReadFailed(logger, ex);
            return Result<ExcelToJsonMultiSheetResponse>.Failure($"No se pudo leer el Excel: {ex.Message}");
        }

        return Result<ExcelToJsonMultiSheetResponse>.Success(new ExcelToJsonMultiSheetResponse(sheets));
    }
}
