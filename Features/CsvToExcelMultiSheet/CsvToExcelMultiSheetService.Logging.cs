using Microsoft.Extensions.Logging;

namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public static partial class CsvToExcelMultiSheetServiceLogging
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error al parsear el CSV de la hoja '{SheetName}'")]
    public static partial void CsvParsingFailed(ILogger logger, string sheetName, Exception ex);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Error al generar el archivo Excel multi-hoja desde CSV")]
    public static partial void ExcelGenerationFailed(ILogger logger, Exception ex);
}
