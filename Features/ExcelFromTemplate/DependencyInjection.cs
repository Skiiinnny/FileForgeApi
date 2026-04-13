namespace FileForgeApi.Features.ExcelFromTemplate;

public static class DependencyInjection
{
    public static IServiceCollection AddExcelFromTemplate(this IServiceCollection services)
    {
        services.AddScoped<IExcelFromTemplateService, ExcelFromTemplateService>();
        return services;
    }
}
