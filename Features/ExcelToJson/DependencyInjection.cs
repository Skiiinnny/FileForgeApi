namespace FileForgeApi.Features.ExcelToJson;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelToJson(this IServiceCollection services)
    {
        services.AddScoped<IExcelToJsonService, ExcelToJsonService>();
        return services;
    }
}
