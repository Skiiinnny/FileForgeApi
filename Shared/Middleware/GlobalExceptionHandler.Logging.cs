using Microsoft.Extensions.Logging;

namespace FileForgeApi.Shared.Middleware;

public static partial class GlobalExceptionHandlerLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Excepción no controlada: {Message}")]
    public static partial void UnhandledException(
        ILogger logger,
        Exception ex,
        string message);
}
