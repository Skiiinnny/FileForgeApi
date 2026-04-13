using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.CsvToExcel;

public static partial class CsvToExcelServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al convertir CSV a Excel con MiniExcel")]
    public static partial void CsvToExcelConversionFailed(ILogger logger, Exception ex);
}
