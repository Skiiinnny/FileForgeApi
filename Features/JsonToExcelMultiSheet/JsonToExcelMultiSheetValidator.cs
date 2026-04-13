using System.Text.Json;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public static class JsonToExcelMultiSheetValidator
{
    public static Result<Dictionary<string, List<Dictionary<string, JsonElement>>>> Validate(
        JsonToExcelMultiSheetRequest? request)
    {
        if (request is null)
            return Result<Dictionary<string, List<Dictionary<string, JsonElement>>>>.Failure(
                "El cuerpo de la petición es obligatorio (JSON con Sheets).");

        if (request.Sheets is null || request.Sheets.Count == 0)
            return Result<Dictionary<string, List<Dictionary<string, JsonElement>>>>.Failure(
                "El diccionario de hojas (Sheets) es obligatorio y no puede estar vacío.");

        foreach (var (sheetName, rows) in request.Sheets)
        {
            if (rows is null || rows.Count == 0)
                return Result<Dictionary<string, List<Dictionary<string, JsonElement>>>>.Failure(
                    $"La hoja '{sheetName}' no tiene filas.");

            bool hasAnyKey = false;
            foreach (var row in rows)
            {
                if (row.Count > 0)
                {
                    hasAnyKey = true;
                    break;
                }
            }

            if (!hasAnyKey)
                return Result<Dictionary<string, List<Dictionary<string, JsonElement>>>>.Failure(
                    $"La hoja '{sheetName}' debe tener al menos una fila con claves para generar encabezados.");
        }

        return Result<Dictionary<string, List<Dictionary<string, JsonElement>>>>.Success(request.Sheets);
    }
}
