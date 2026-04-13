namespace FileForgeApi.Features.ExcelToCsv;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelToCsv(this IServiceCollection services)
    {
        services.AddScoped<IExcelToCsvService, ExcelToCsvService>();
        return services;
    }
}
