using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelAppendRows;

public static partial class ExcelAppendRowsServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al añadir filas al archivo Excel")]
    public static partial void AppendRowsFailed(ILogger logger, Exception ex);
}
