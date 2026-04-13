using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelToJson;

public static class ExcelToJsonEndpoint
{
    public static void MapExcelToJson(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/to-json", ConvertHandler)
            .WithName("ExcelToJson")
            .WithTags("Excel")
            .WithDescription("Convierte un archivo Excel codificado en Base64 a formato JSON. Cada fila del Excel se transforma en un diccionario clave-valor usando los encabezados como claves.")
            .Accepts<ExcelToJsonRequest>("application/json")
            .Produces<ExcelToJsonResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] ExcelToJsonRequest? request,
        IExcelToJsonService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
