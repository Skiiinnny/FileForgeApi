using System.Text.Json;
using FileForgeApi.Shared.Encoding;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.JsonToCsv;

public static class JsonToCsvValidator
{
    public static Result<List<Dictionary<string, JsonElement>>> Validate(JsonToCsvRequest? request)
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

        if (request.Separator is { Length: not 1 })
            return Result<List<Dictionary<string, JsonElement>>>.Failure("Separator debe ser un único carácter.");

        if (request.Encoding is { Length: > 0 } && !EncodingHelper.TryGetEncoding(request.Encoding, out _))
            return Result<List<Dictionary<string, JsonElement>>>.Failure($"Encoding no válido: {request.Encoding}");

        if (request.NewLine is { Length: > 0 } && request.NewLine is not "\n" and not "\r\n")
            return Result<List<Dictionary<string, JsonElement>>>.Failure("NewLine debe ser \"\\n\" o \"\\r\\n\".");

        return Result<List<Dictionary<string, JsonElement>>>.Success(request.Rows);
    }
}
