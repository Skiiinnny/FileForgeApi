using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public static class CsvToExcelMultiSheetEndpoint
{
    public static void MapCsvToExcelMultiSheet(this IEndpointRouteBuilder app)
    {
        app.MapPost("/csv-to-excel-multi-sheet", ConvertHandler)
            .WithName("CsvToExcelMultiSheet")
            .WithTags("Csv")
            .WithDescription("Convierte múltiples archivos CSV codificados en Base64 a un único Excel multi-hoja. Cada clave del objeto sheets es el nombre de una hoja y el valor es el CSV en Base64.")
            .Accepts<CsvToExcelMultiSheetRequest>("application/json")
            .Produces<CsvToExcelMultiSheetResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] CsvToExcelMultiSheetRequest? request,
        ICsvToExcelMultiSheetService service,
        [FromQuery] string? separator = ",",
        [FromQuery] string? encoding = "utf-8")
    {
        var result = await service.ConvertAsync(request, separator, encoding);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
