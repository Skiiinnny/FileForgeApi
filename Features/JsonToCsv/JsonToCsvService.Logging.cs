using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.JsonToCsv;

public static partial class JsonToCsvServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al generar el archivo CSV con MiniExcel")]
    public static partial void CsvGenerationFailed(ILogger logger, Exception ex);
}
