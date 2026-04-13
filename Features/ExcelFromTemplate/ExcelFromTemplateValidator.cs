using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelFromTemplate;

public static class ExcelFromTemplateValidator
{
    public static Result<(byte[]? TemplateBytes, bool UseUrl, List<Dictionary<string, string>> Rows)> Validate(ExcelFromTemplateRequest? request)
    {
        if (request is null)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("El cuerpo de la petición es obligatorio.");

        bool hasBase64 = !string.IsNullOrWhiteSpace(request.Base64Template);
        bool hasUrl = !string.IsNullOrWhiteSpace(request.TemplateUrl);

        if (hasBase64 && hasUrl)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("No se pueden proporcionar Base64Template y TemplateUrl a la vez. Proporcione exactamente uno de los dos.");

        if (!hasBase64 && !hasUrl)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("Se debe proporcionar exactamente uno de los dos campos: Base64Template o TemplateUrl.");

        if (request.Rows is null || request.Rows.Count == 0)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("La lista de filas (Rows) es obligatoria y no puede estar vacía.");

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
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("Al menos una fila debe contener al menos una clave para generar encabezados o coincidir con la plantilla.");

        if (hasUrl)
        {
            if (!Uri.TryCreate(request.TemplateUrl, UriKind.Absolute, out _))
                return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("TemplateUrl no es una URI absoluta válida.");

            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Success((null, true, request.Rows));
        }

        byte[] templateBytes;
        try
        {
            templateBytes = Convert.FromBase64String(request.Base64Template!.Trim());
        }
        catch (FormatException)
        {
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("Base64Template no es una cadena Base64 válida.");
        }

        if (templateBytes.Length == 0)
            return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Failure("La plantilla está vacía.");

        return Result<(byte[]?, bool, List<Dictionary<string, string>>)>.Success((templateBytes, false, request.Rows));
    }
}
