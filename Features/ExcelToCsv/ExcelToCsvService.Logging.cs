using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelToCsv;

public static partial class ExcelToCsvServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al convertir Excel a CSV con MiniExcel")]
    public static partial void ExcelToCsvConversionFailed(ILogger logger, Exception ex);
}
