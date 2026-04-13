using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.CsvToExcel;

public static class CsvToExcelEndpoint
{
    public static void MapCsvToExcel(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/csv/to-excel", ConvertHandler)
            .WithName("CsvToExcel")
            .WithTags("Csv")
            .WithDescription("Convierte un archivo CSV codificado en Base64 a Excel. Opcionalmente permite especificar separador y encoding del CSV de entrada.")
            .Accepts<CsvToExcelRequest>("application/json")
            .Produces<CsvToExcelResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] CsvToExcelRequest? request,
        ICsvToExcelService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
