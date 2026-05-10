using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.Base64ToExcel;

public interface IBase64ToExcelService
{
    Task<Result<BinaryFilePayload>> ConvertAsync(Base64ToExcelRequest? request);
}
