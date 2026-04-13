namespace FileForgeApi.Features.Base64ToJson;

public interface IBase64ToJsonService
{
    Task<IResult> ConvertAsync(Base64ToJsonRequest? request);
}
