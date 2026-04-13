using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelAppendRows;

public static class ExcelAppendRowsEndpoint
{
    public static void MapExcelAppendRows(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/append-rows", AppendRowsHandler)
            .WithName("ExcelAppendRows")
            .WithTags("Excel")
            .WithDescription("Añade filas al final de un archivo Excel existente. Devuelve el Excel actualizado en Base64.")
            .Accepts<ExcelAppendRowsRequest>("application/json")
            .Produces<ExcelAppendRowsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> AppendRowsHandler(
        [FromBody] ExcelAppendRowsRequest? request,
        IExcelAppendRowsService service)
    {
        var result = await service.AppendRowsAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
