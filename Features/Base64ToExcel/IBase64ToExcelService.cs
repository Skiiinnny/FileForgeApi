namespace FileForgeApi.Features.Base64ToExcel;

public interface IBase64ToExcelService
{
    Task<IResult> ConvertAsync(Base64ToExcelRequest? request);
}
