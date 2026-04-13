using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelFromTemplate;

public static class ExcelFromTemplateEndpoint
{
    public static void MapExcelFromTemplate(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/from-template", MergeFromTemplateHandler)
            .WithName("ExcelFromTemplate")
            .WithTags("Excel")
            .WithDescription("Fusiona datos JSON en una plantilla Excel (Base64). La plantilla puede estar vacía o contener encabezados/datos. Devuelve el Excel resultante en Base64.")
            .Accepts<ExcelFromTemplateRequest>("application/json")
            .Produces<ExcelFromTemplateResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> MergeFromTemplateHandler(
        [FromBody] ExcelFromTemplateRequest? request,
        IExcelFromTemplateService service)
    {
        var result = await service.MergeFromTemplateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
