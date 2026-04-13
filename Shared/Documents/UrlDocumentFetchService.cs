using System.Net;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Shared.Documents;

public sealed class UrlDocumentFetchService(HttpClient httpClient) : IDocumentFetchService
{
    private const long MaxResponseSizeBytes = 50 * 1024 * 1024; // 50 MB

    public async Task<Result<byte[]>> FetchAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return Result<byte[]>.Failure("La URL no es válida.");

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return Result<byte[]>.Failure("Solo se permiten URLs con esquema HTTP o HTTPS.");

        if (IsPrivateOrReservedHost(uri.Host))
            return Result<byte[]>.Failure("La URL apunta a una dirección no permitida.");

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (TaskCanceledException)
        {
            return Result<byte[]>.Failure("La descarga del documento excedió el tiempo límite.");
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failure($"Error al descargar el documento: {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
            return Result<byte[]>.Failure($"La URL retornó el código HTTP {(int)response.StatusCode}.");

        if (response.Content.Headers.ContentLength > MaxResponseSizeBytes)
            return Result<byte[]>.Failure("El documento supera el tamaño máximo permitido (50 MB).");

        byte[] bytes;
        try
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var memStream = new MemoryStream();
            var buffer = new byte[81920];
            int read;
            long totalRead = 0;

            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                totalRead += read;
                if (totalRead > MaxResponseSizeBytes)
                    return Result<byte[]>.Failure("El documento supera el tamaño máximo permitido (50 MB).");
                await memStream.WriteAsync(buffer.AsMemory(0, read));
            }

            bytes = memStream.ToArray();
        }
        catch (TaskCanceledException)
        {
            return Result<byte[]>.Failure("La descarga del documento excedió el tiempo límite.");
        }

        if (bytes.Length == 0)
            return Result<byte[]>.Failure("El documento descargado está vacío.");

        return Result<byte[]>.Success(bytes);
    }

    private static bool IsPrivateOrReservedHost(string host)
    {
        if (!IPAddress.TryParse(host, out var ip))
            return false;

        var bytes = ip.GetAddressBytes();

        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // 127.0.0.0/8
            if (bytes[0] == 127) return true;
            // 10.0.0.0/8
            if (bytes[0] == 10) return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168) return true;
            // 169.254.0.0/16 (link-local)
            if (bytes[0] == 169 && bytes[1] == 254) return true;
        }

        return false;
    }
}
