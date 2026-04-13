using FileForgeApi.Features.ExcelAppendRows;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.ExcelAppendRows;

public class ExcelAppendRowsServiceTests
{
    private readonly ILogger<ExcelAppendRowsService> _logger = Substitute.For<ILogger<ExcelAppendRowsService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelAppendRowsService _sut;

    private static readonly List<Dictionary<string, string>> SomeRows =
        new() { new() { ["A"] = "1", ["B"] = "2" } };

    public ExcelAppendRowsServiceTests()
    {
        _sut = new ExcelAppendRowsService(_logger, _fetchService);
    }

    [Fact]
    public async Task AppendRowsAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.AppendRowsAsync(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public async Task AppendRowsAsync_EmptyBase64_ReturnsFailure()
    {
        var result = await _sut.AppendRowsAsync(new ExcelAppendRowsRequest("", SomeRows));

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public async Task AppendRowsAsync_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = await _sut.AppendRowsAsync(new ExcelAppendRowsRequest(
            Convert.ToBase64String(bytes), SomeRows, "https://example.com/f.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public async Task AppendRowsAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var result = await _sut.AppendRowsAsync(new ExcelAppendRowsRequest(null, SomeRows, "https://example.com/notfound.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    [Fact]
    public async Task AppendRowsAsync_DocumentUrlSuccess_ReturnsExcel()
    {
        var excelBytes = CreateTestExcelBytes(new[] { new Dictionary<string, object> { ["A"] = "existing" } });
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(excelBytes));

        var result = await _sut.AppendRowsAsync(new ExcelAppendRowsRequest(null, SomeRows, "https://example.com/file.xlsx"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task AppendRowsAsync_NullRows_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = await _sut.AppendRowsAsync(new ExcelAppendRowsRequest(Convert.ToBase64String(bytes), null!));

        Assert.False(result.IsSuccess);
        Assert.Contains("Rows", result.Error!);
    }

    private static byte[] CreateTestExcelBytes(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return stream.ToArray();
    }
}
