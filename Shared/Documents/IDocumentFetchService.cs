using FileForgeApi.Shared.Results;

namespace FileForgeApi.Shared.Documents;

public interface IDocumentFetchService
{
    Task<Result<byte[]>> FetchAsync(string url);
}
