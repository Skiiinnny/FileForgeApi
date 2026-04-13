namespace FileForgeApi.Features.CsvToJson;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvToJson(this IServiceCollection services)
    {
        services.AddScoped<ICsvToJsonService, CsvToJsonService>();
        return services;
    }
}
