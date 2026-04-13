using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.JsonToExcel;

public static class JsonToExcelEndpoint
{
    public static void MapJsonToExcel(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/json/to-excel", ConvertHandler)
            .WithName("JsonToExcel")
            .WithTags("Json")
            .WithDescription("Convierte datos JSON (lista de diccionarios clave-valor) a un archivo Excel codificado en Base64.")
            .Accepts<JsonToExcelRequest>("application/json")
            .Produces<JsonToExcelResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] JsonToExcelRequest? request,
        IJsonToExcelService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
