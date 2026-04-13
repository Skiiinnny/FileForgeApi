using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public static partial class JsonToExcelMultiSheetServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al generar el archivo Excel con hojas múltiples")]
    public static partial void ExcelMultiSheetGenerationFailed(ILogger logger, Exception ex);
}
