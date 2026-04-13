using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public static class JsonToExcelMultiSheetEndpoint
{
    public static void MapJsonToExcelMultiSheet(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/json/to-excel/multi-sheet", ConvertHandler)
            .WithName("JsonToExcelMultiSheet")
            .WithTags("Json")
            .WithDescription("Convierte datos JSON organizados por hojas a un archivo Excel codificado en Base64. Cada clave del objeto sheets representa el nombre de una hoja.")
            .Accepts<JsonToExcelMultiSheetRequest>("application/json")
            .Produces<JsonToExcelMultiSheetResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] JsonToExcelMultiSheetRequest? request,
        IJsonToExcelMultiSheetService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
