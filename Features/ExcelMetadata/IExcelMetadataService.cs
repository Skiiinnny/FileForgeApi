using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelMetadata;

public interface IExcelMetadataService
{
    Task<Result<ExcelMetadataResponse>> GetMetadataAsync(ExcelMetadataRequest? request);
}
