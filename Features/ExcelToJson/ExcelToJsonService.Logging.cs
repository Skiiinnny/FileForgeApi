using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelToJson;

public static partial class ExcelToJsonServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al leer el archivo Excel con MiniExcel")]
    public static partial void ExcelReadFailed(ILogger logger, Exception ex);
}
