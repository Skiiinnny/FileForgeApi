using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelMetadata;

public static partial class ExcelMetadataServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al extraer metadatos del archivo Excel")]
    public static partial void MetadataExtractionFailed(ILogger logger, Exception ex);
}
