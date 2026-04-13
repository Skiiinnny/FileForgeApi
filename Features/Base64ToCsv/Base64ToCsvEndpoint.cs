using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.Base64ToCsv;

public static class Base64ToCsvEndpoint
{
    public static void MapBase64ToCsv(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/base64/to-csv", ConvertHandler)
            .WithName("Base64ToCsv")
            .WithTags("Base64")
            .WithDescription("Decodifica una cadena Base64 y retorna el contenido como archivo CSV descargable.")
            .Accepts<Base64ToCsvRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] Base64ToCsvRequest? request,
        IBase64ToCsvService service)
    {
        return await service.ConvertAsync(request);
    }
}
