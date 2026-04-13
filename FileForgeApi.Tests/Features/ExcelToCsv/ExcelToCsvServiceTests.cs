using FileForgeApi.Features.ExcelToCsv;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;
using System.Text;

namespace FileForgeApi.Tests.Features.ExcelToCsv;

public class ExcelToCsvServiceTests
{
    private readonly ILogger<ExcelToCsvService> _logger = Substitute.For<ILogger<ExcelToCsvService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelToCsvService _sut;

    public ExcelToCsvServiceTests()
    {
        _sut = new ExcelToCsvService(_logger, _fetchService);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptyBase64_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest("");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_InvalidExcelContent_ReturnsFailure()
    {
        var invalidBytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToCsvRequest(Convert.ToBase64String(invalidBytes));

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo convertir", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_ValidExcel_WithoutOptions_ReturnsCsvBase64()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 },
            new Dictionary<string, object> { ["Nombre"] = "Bob", ["Edad"] = 25 }
        });
        var request = new ExcelToCsvRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value!.Base64Content));

        var csvBytes = Convert.FromBase64String(result.Value.Base64Content);
        var csv = Encoding.UTF8.GetString(csvBytes);
        Assert.Contains("Nombre", csv);
        Assert.Contains("Alice", csv);
        Assert.Contains("Bob", csv);
    }

    [Fact]
    public async Task ConvertAsync_ValidExcel_WithSeparatorSemicolon_ReturnsCsvWithSemicolon()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        var request = new ExcelToCsvRequest(base64, Separator: ";");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        var csvBytes = Convert.FromBase64String(result.Value!.Base64Content);
        var csv = Encoding.UTF8.GetString(csvBytes);
        Assert.Contains(";", csv);
    }

    [Fact]
    public async Task ConvertAsync_InvalidEncoding_ReturnsFailure()
    {
        var base64 = CreateTestExcelBase64(new[] { new Dictionary<string, object> { ["A"] = 1 } });
        var request = new ExcelToCsvRequest(base64, Encoding: "invalid-encoding-xyz");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_InvalidSeparator_ReturnsFailure()
    {
        var base64 = CreateTestExcelBase64(new[] { new Dictionary<string, object> { ["A"] = 1 } });
        var request = new ExcelToCsvRequest(base64, Separator: ";;");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Separator", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlSuccess_ReturnsCsv()
    {
        var excelBytes = CreateTestExcelBytes(new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(excelBytes));

        var request = new ExcelToCsvRequest(null, DocumentUrl: "https://example.com/file.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var request = new ExcelToCsvRequest(null, DocumentUrl: "https://example.com/notfound.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    private static string CreateTestExcelBase64(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return Convert.ToBase64String(stream.ToArray());
    }

    private static byte[] CreateTestExcelBytes(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return stream.ToArray();
    }
}
