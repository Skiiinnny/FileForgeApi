using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.JsonToExcel;

public static partial class JsonToExcelServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al generar el archivo Excel con MiniExcel")]
    public static partial void ExcelGenerationFailed(ILogger logger, Exception ex);
}
