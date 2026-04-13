using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelFromTemplate;

public interface IExcelFromTemplateService
{
    Task<Result<ExcelFromTemplateResponse>> MergeFromTemplateAsync(ExcelFromTemplateRequest? request);
}
