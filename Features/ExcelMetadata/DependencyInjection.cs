namespace FileForgeApi.Features.ExcelMetadata;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelMetadata(this IServiceCollection services)
    {
        services.AddScoped<IExcelMetadataService, ExcelMetadataService>();
        return services;
    }
}
