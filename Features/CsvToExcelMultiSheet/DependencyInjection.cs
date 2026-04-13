namespace FileForgeApi.Features.CsvToExcelMultiSheet;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvToExcelMultiSheet(this IServiceCollection services)
    {
        services.AddScoped<ICsvToExcelMultiSheetService, CsvToExcelMultiSheetService>();
        return services;
    }
}
