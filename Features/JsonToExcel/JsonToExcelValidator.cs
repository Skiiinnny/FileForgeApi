using System.Text.Json;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToExcel;

public static class JsonToExcelValidator
{
    public static Result<List<Dictionary<string, JsonElement>>> Validate(JsonToExcelRequest? request)
    {
        if (request is null)
            return Result<List<Dictionary<string, JsonElement>>>.Failure("El cuerpo de la petición es obligatorio (JSON con Rows).");

        if (request.Rows is null || request.Rows.Count == 0)
            return Result<List<Dictionary<string, JsonElement>>>.Failure("La lista de filas (Rows) es obligatoria y no puede estar vacía.");

        bool hasAnyKey = false;
        foreach (var row in request.Rows)
        {
            if (row.Count > 0)
            {
                hasAnyKey = true;
                break;
            }
        }

        if (!hasAnyKey)
            return Result<List<Dictionary<string, JsonElement>>>.Failure("Al menos una fila debe contener al menos una clave para generar encabezados.");

        return Result<List<Dictionary<string, JsonElement>>>.Success(request.Rows);
    }
}
