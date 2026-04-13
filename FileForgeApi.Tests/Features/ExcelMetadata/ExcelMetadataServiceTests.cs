using FileForgeApi.Features.ExcelMetadata;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FileForgeApi.Tests.Features.ExcelMetadata;

public class ExcelMetadataServiceTests
{
    private readonly ILogger<ExcelMetadataService> _logger = Substitute.For<ILogger<ExcelMetadataService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelMetadataService _sut;

    public ExcelMetadataServiceTests()
    {
        _sut = new ExcelMetadataService(_logger, _fetchService);
    }

    [Fact]
    public async Task GetMetadataAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.GetMetadataAsync(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_EmptyBase64_ReturnsFailure()
    {
        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest(""));

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_InvalidBase64_ReturnsFailure()
    {
        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest("!!!invalid!!!"));

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64 válida", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_InvalidExcelBytes_ReturnsFailure()
    {
        var invalidBytes = new byte[] { 1, 2, 3, 4 };
        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest(Convert.ToBase64String(invalidBytes)));

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo extraer metadatos", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_DocumentUrlSuccess_ReturnsMetadata()
    {
        var invalidBytes = new byte[] { 1, 2, 3, 4 };
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(invalidBytes));

        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest(null, "https://example.com/file.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo extraer metadatos", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest(null, "https://example.com/notfound.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    [Fact]
    public async Task GetMetadataAsync_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = await _sut.GetMetadataAsync(new ExcelMetadataRequest(Convert.ToBase64String(bytes), "https://example.com/f.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }
}
