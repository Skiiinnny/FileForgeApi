namespace FileForgeApi.Features.JsonToExcelMultiSheet;

public static class DependencyInjection
{
    public static IServiceCollection AddJsonToExcelMultiSheet(this IServiceCollection services)
    {
        services.AddScoped<IJsonToExcelMultiSheetService, JsonToExcelMultiSheetService>();
        return services;
    }
}
