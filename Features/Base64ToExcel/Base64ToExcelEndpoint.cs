using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.Base64ToExcel;

public static class Base64ToExcelEndpoint
{
    public static void MapBase64ToExcel(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/base64/to-excel", ConvertHandler)
            .WithName("Base64ToExcel")
            .WithTags("Base64")
            .WithDescription("Decodifica una cadena Base64 y retorna el contenido como archivo Excel (.xlsx) descargable.")
            .Accepts<Base64ToExcelRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> ConvertHandler(
        [FromBody] Base64ToExcelRequest? request,
        IBase64ToExcelService service)
    {
        return await service.ConvertAsync(request);
    }
}
