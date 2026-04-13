using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelAppendRows;

public static class ExcelAppendRowsValidator
{
    public static Result<(byte[]? FileBytes, bool UseUrl, List<Dictionary<string, string>> Rows)> Validate(ExcelAppendRowsRequest? request)
    {
        if (request is null)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("El cuerpo de la petición es obligatorio.");

        bool hasBase64 = !string.IsNullOrWhiteSpace(request.Base64Content);
        bool hasUrl = !string.IsNullOrWhiteSpace(request.DocumentUrl);

        if (hasBase64 && hasUrl)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("No se pueden proporcionar Base64Content y DocumentUrl a la vez. Proporcione exactamente uno de los dos.");

        if (!hasBase64 && !hasUrl)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("Se debe proporcionar exactamente uno de los dos campos: Base64Content o DocumentUrl.");

        if (request.Rows is null || request.Rows.Count == 0)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("La lista de filas (Rows) es obligatoria y no puede estar vacía.");

        if (hasUrl)
        {
            if (!Uri.TryCreate(request.DocumentUrl, UriKind.Absolute, out _))
                return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("DocumentUrl no es una URI absoluta válida.");

            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Success((null, true, request.Rows));
        }

        byte[] fileBytes;
        try
        {
            fileBytes = Convert.FromBase64String(request.Base64Content!.Trim());
        }
        catch (FormatException)
        {
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("Base64Content no es una cadena Base64 válida.");
        }

        if (fileBytes.Length == 0)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("El archivo está vacío.");

        return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Success((fileBytes, false, request.Rows));
    }
}
