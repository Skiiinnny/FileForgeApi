using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelMetadata;

public static class ExcelMetadataValidator
{
    public static Result<(byte[]? FileBytes, bool UseUrl)> Validate(ExcelMetadataRequest? request)
    {
        if (request is null)
            return Result<(byte[]?, bool)>.Failure("El cuerpo de la petición es obligatorio.");

        bool hasBase64 = !string.IsNullOrWhiteSpace(request.Base64Content);
        bool hasUrl = !string.IsNullOrWhiteSpace(request.DocumentUrl);

        if (hasBase64 && hasUrl)
            return Result<(byte[]?, bool)>.Failure("No se pueden proporcionar Base64Content y DocumentUrl a la vez. Proporcione exactamente uno de los dos.");

        if (!hasBase64 && !hasUrl)
            return Result<(byte[]?, bool)>.Failure("Se debe proporcionar exactamente uno de los dos campos: Base64Content o DocumentUrl.");

        if (hasUrl)
        {
            if (!Uri.TryCreate(request.DocumentUrl, UriKind.Absolute, out _))
                return Result<(byte[]?, bool)>.Failure("DocumentUrl no es una URI absoluta válida.");

            return Result<(byte[]?, bool)>.Success((null, true));
        }

        byte[] fileBytes;
        try
        {
            fileBytes = Convert.FromBase64String(request.Base64Content!.Trim());
        }
        catch (FormatException)
        {
            return Result<(byte[]?, bool)>.Failure("Base64Content no es una cadena Base64 válida.");
        }

        if (fileBytes.Length == 0)
            return Result<(byte[]?, bool)>.Failure("El archivo está vacío.");

        return Result<(byte[]?, bool)>.Success((fileBytes, false));
    }
}
