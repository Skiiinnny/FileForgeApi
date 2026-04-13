using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelToCsv;

public static class ExcelToCsvEndpoint
{
    public static void MapExcelToCsv(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/to-csv", ConvertHandler)
            .WithName("ExcelToCsv")
            .WithTags("Excel")
            .WithDescription("Convierte un archivo Excel codificado en Base64 a CSV. Opcionalmente permite especificar separador, encoding y salto de línea para el CSV resultante.")
            .Accepts<ExcelToCsvRequest>("application/json")
            .Produces<ExcelToCsvResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] ExcelToCsvRequest? request,
        IExcelToCsvService service)
    {
        var result = await service.ConvertAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
