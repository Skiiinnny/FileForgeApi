namespace FileForgeApi.Features.Base64ToExcel;

public static partial class Base64ToExcelServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Base64ToExcel: conversión solicitada, filename={Filename}")]
    public static partial void ConvertRequested(ILogger logger, string? filename);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Base64ToExcel: validación fallida — {Error}")]
    public static partial void ValidationFailed(ILogger logger, string error);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Base64ToExcel: conversión exitosa, bytes={Bytes}")]
    public static partial void ConvertSucceeded(ILogger logger, int bytes);
}
