using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public static class CsvToExcelMultiSheetValidator
{
    public static Result<bool> Validate(CsvToExcelMultiSheetRequest? request)
    {
        if (request is null)
            return Result<bool>.Failure("El cuerpo de la petición es obligatorio (JSON con Sheets).");

        if (request.Sheets is null || request.Sheets.Count == 0)
            return Result<bool>.Failure("El diccionario de hojas (Sheets) es obligatorio y debe contener al menos una entrada.");

        if (request.Separator is { Length: not 1 })
            return Result<bool>.Failure("Separator debe ser un único carácter.");

        if (request.Encoding is { Length: > 0 } && !EncodingHelper.TryGetEncoding(request.Encoding, out _))
            return Result<bool>.Failure($"Encoding no válido: {request.Encoding}");

        foreach (var (sheetName, csvBase64) in request.Sheets)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
                return Result<bool>.Failure("El nombre de hoja no puede estar vacío o contener solo espacios.");

            if (sheetName.Length > 31)
                return Result<bool>.Failure(
                    $"El nombre de hoja '{sheetName}' excede el límite de 31 caracteres de Excel.");

            if (string.IsNullOrWhiteSpace(csvBase64))
                return Result<bool>.Failure($"El contenido CSV de la hoja '{sheetName}' no puede estar vacío.");
        }

        return Result<bool>.Success(true);
    }
}
