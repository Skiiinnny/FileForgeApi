namespace FileForgeApi.Features.Base64ToCsv;

public static partial class Base64ToCsvServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Base64ToCsv: conversión solicitada, filename={Filename}")]
    public static partial void ConvertRequested(ILogger logger, string? filename);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Base64ToCsv: validación fallida — {Error}")]
    public static partial void ValidationFailed(ILogger logger, string error);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Base64ToCsv: conversión exitosa, bytes={Bytes}")]
    public static partial void ConvertSucceeded(ILogger logger, int bytes);
}
