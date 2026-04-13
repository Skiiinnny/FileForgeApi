using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.JsonToCsv;

public static class JsonToCsvEndpoint
{
    public static void MapJsonToCsv(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/json/to-csv", ConvertHandler)
            .WithName("JsonToCsv")
            .WithTags("Json")
            .WithDescription("Convierte datos JSON (lista de diccionarios clave-valor) a un archivo CSV codificado en Base64.")
            .Accepts<JsonToCsvRequest>("application/json")
            .Produces<JsonToCsvResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] JsonToCsvRequest? request,
        IJsonToCsvService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
