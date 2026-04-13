namespace FileForgeApi.Features.Base64ToCsv;

public interface IBase64ToCsvService
{
    Task<IResult> ConvertAsync(Base64ToCsvRequest? request);
}
