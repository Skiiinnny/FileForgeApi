using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.CsvToJson;

public static class CsvToJsonEndpoint
{
    public static void MapCsvToJson(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/csv/to-json", ConvertHandler)
            .WithName("CsvToJson")
            .WithTags("Csv")
            .WithDescription("Convierte un archivo CSV codificado en Base64 a formato JSON. Cada fila del CSV se transforma en un diccionario clave-valor usando la primera fila como encabezados.")
            .Accepts<CsvToJsonRequest>("application/json")
            .Produces<CsvToJsonResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] CsvToJsonRequest? request,
        ICsvToJsonService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
