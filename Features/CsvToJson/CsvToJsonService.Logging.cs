using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.CsvToJson;

public static partial class CsvToJsonServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al leer el archivo CSV con MiniExcel")]
    public static partial void CsvReadFailed(ILogger logger, Exception ex);
}
