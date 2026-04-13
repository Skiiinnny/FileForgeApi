using Microsoft.AspNetCore.Mvc;

namespace FileForgeApi.Features.ExcelMetadata;

public static class ExcelMetadataEndpoint
{
    public static void MapExcelMetadata(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/excel/metadata", GetMetadataHandler)
            .WithName("ExcelMetadata")
            .WithTags("Excel")
            .WithDescription("Extrae las propiedades del documento Excel (creator, created, modified, lastModifiedBy, title, subject, keywords, description).")
            .Accepts<ExcelMetadataRequest>("application/json")
            .Produces<ExcelMetadataResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetMetadataHandler(
        [FromBody] ExcelMetadataRequest? request,
        IExcelMetadataService service)
    {
        var result = await service.GetMetadataAsync(request);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
