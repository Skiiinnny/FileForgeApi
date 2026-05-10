using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.Base64ToJson;

public static class Base64ToJsonEndpoint
{
    public static void MapBase64ToJson(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/base64/to-json", ConvertHandler)
            .WithName("Base64ToJson")
            .WithTags("Base64")
            .WithDescription("Decodifica una cadena Base64 y retorna el contenido como archivo JSON descargable.")
            .Accepts<Base64ToJsonRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] Base64ToJsonRequest? request,
        IBase64ToJsonService service)
    {
        var result = await service.ConvertAsync(request);

        if (!result.IsSuccess)
            return Results.BadRequest(new { error = result.Error });

        var file = result.Value!;
        return Results.File(file.Content, file.ContentType, file.DownloadFileName);
    }
}
