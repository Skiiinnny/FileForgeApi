namespace FileForgeApi.Features.CsvToExcel;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvToExcel(this IServiceCollection services)
    {
        services.AddScoped<ICsvToExcelService, CsvToExcelService>();
        return services;
    }
}
