using FileForgeApi.Shared.Json;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;

namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public sealed class JsonToExcelMultiSheetService(ILogger<JsonToExcelMultiSheetService> logger)
    : IJsonToExcelMultiSheetService
{
    public async Task<Result<JsonToExcelMultiSheetResponse>> ConvertAsync(JsonToExcelMultiSheetRequest? request)
    {
        var validation = JsonToExcelMultiSheetValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<JsonToExcelMultiSheetResponse>.Failure(validation.Error!);

        var sheets = validation.Value!;
        var sheetsDict = new Dictionary<string, object>();
        foreach (var (name, rows) in sheets)
        {
            sheetsDict[name] = rows.Select(row =>
                (IDictionary<string, object?>)row.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToClrObject())).ToList();
        }

        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        try
        {
            MiniExcel.SaveAs(tempPath, sheetsDict);

            var excelBytes = await File.ReadAllBytesAsync(tempPath);
            var base64 = Convert.ToBase64String(excelBytes);
            return Result<JsonToExcelMultiSheetResponse>.Success(new JsonToExcelMultiSheetResponse(base64));
        }
        catch (Exception ex)
        {
            JsonToExcelMultiSheetServiceLogging.ExcelMultiSheetGenerationFailed(logger, ex);
            return Result<JsonToExcelMultiSheetResponse>.Failure($"No se pudo generar el Excel: {ex.Message}");
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
