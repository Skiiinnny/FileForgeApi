namespace FileForgeApi.Features.ExcelAppendRows;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelAppendRows(this IServiceCollection services)
    {
        services.AddScoped<IExcelAppendRowsService, ExcelAppendRowsService>();
        return services;
    }
}
