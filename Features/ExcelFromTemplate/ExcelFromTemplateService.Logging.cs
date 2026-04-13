using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.ExcelFromTemplate;

public static partial class ExcelFromTemplateServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al fusionar datos en la plantilla Excel")]
    public static partial void MergeFromTemplateFailed(ILogger logger, Exception ex);
}
