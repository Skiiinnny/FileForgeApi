using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public static partial class ExcelToJsonMultiSheetServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al leer el archivo Excel con hojas múltiples")]
    public static partial void ExcelMultiSheetReadFailed(ILogger logger, Exception ex);
}
