using FileForgeApi.Shared.Json;
using FileForgeApi.Shared.Results;
using MiniExcelLibs;

namespace FileForgeApi.Features.JsonToExcel;

public sealed class JsonToExcelService(ILogger<JsonToExcelService> logger) : IJsonToExcelService
{
    public async Task<Result<JsonToExcelResponse>> ConvertAsync(JsonToExcelRequest? request)
    {
        var validation = JsonToExcelValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<JsonToExcelResponse>.Failure(validation.Error!);

        var rows = validation.Value!;

        var clrRows = rows.Select(row =>
            (IDictionary<string, object?>)row.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToClrObject())).ToList();

        byte[] excelBytes;
        try
        {
            using var stream = new MemoryStream();
            await stream.SaveAsAsync(clrRows);
            excelBytes = stream.ToArray();
        }
        catch (Exception ex)
        {
            JsonToExcelServiceLogging.ExcelGenerationFailed(logger, ex);
            return Result<JsonToExcelResponse>.Failure($"No se pudo generar el Excel: {ex.Message}");
        }

        var base64 = Convert.ToBase64String(excelBytes);
        return Result<JsonToExcelResponse>.Success(new JsonToExcelResponse(base64));
    }
}
