using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public static class ExcelToJsonMultiSheetEndpoint
{
    public static void MapExcelToJsonMultiSheet(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/to-json/multi-sheet", ConvertHandler)
            .WithName("ExcelToJsonMultiSheet")
            .WithTags("Excel")
            .WithDescription("Convierte un archivo Excel codificado en Base64 a formato JSON incluyendo todas las hojas. Cada hoja se devuelve como un objeto con el nombre de la hoja como clave.")
            .Accepts<ExcelToJsonMultiSheetRequest>("application/json")
            .Produces<ExcelToJsonMultiSheetResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] ExcelToJsonMultiSheetRequest? request,
        IExcelToJsonMultiSheetService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
