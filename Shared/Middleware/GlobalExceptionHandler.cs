using Microsoft.AspNetCore.Diagnostics;

namespace FileForgeApi.Shared.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        GlobalExceptionHandlerLogging.UnhandledException(logger, exception, exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        const string body = """{"error":"Error interno del servidor."}""";
        await httpContext.Response.WriteAsync(body, cancellationToken);

        return true;
    }
}
