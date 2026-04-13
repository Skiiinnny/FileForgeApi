using FileForgeApi.Features.ExcelFromTemplate;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.ExcelFromTemplate;

public class ExcelFromTemplateServiceTests
{
    private readonly ILogger<ExcelFromTemplateService> _logger = Substitute.For<ILogger<ExcelFromTemplateService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelFromTemplateService _sut;

    private static readonly List<Dictionary<string, string>> SomeRows =
        new() { new() { ["A"] = "1", ["B"] = "2" } };

    public ExcelFromTemplateServiceTests()
    {
        _sut = new ExcelFromTemplateService(_logger, _fetchService);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.MergeFromTemplateAsync(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_EmptyBase64Template_ReturnsFailure()
    {
        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest("", SomeRows));

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest(
            Convert.ToBase64String(bytes), SomeRows, "https://example.com/template.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_NullRows_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest(Convert.ToBase64String(bytes), null!));

        Assert.False(result.IsSuccess);
        Assert.Contains("Rows", result.Error!);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_TemplateUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest(null, SomeRows, "https://example.com/notfound.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    [Fact]
    public async Task MergeFromTemplateAsync_TemplateUrlSuccess_ReturnsMergedExcel()
    {
        var templateBytes = CreateTestExcelBytes(new[] { new Dictionary<string, object> { ["A"] = "existing" } });
        _fetchService.FetchAsync("https://example.com/template.xlsx")
            .Returns(Result<byte[]>.Success(templateBytes));

        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest(null, SomeRows, "https://example.com/template.xlsx"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value!.Base64Content));
    }

    [Fact]
    public async Task MergeFromTemplateAsync_InvalidUrl_ReturnsFailure()
    {
        var result = await _sut.MergeFromTemplateAsync(new ExcelFromTemplateRequest(null, SomeRows, "not-a-url"));

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }

    private static byte[] CreateTestExcelBytes(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return stream.ToArray();
    }
}
