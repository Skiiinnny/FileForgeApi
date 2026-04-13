namespace FileForgeApi.Features.ExcelToJsonMultiSheet;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelToJsonMultiSheet(this IServiceCollection services)
    {
        services.AddScoped<IExcelToJsonMultiSheetService, ExcelToJsonMultiSheetService>();
        return services;
    }
}
