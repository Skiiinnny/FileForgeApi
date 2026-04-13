using FileForgeApi.Features.CsvToExcel;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.CsvToExcel;

public class CsvToExcelServiceTests
{
    private readonly ILogger<CsvToExcelService> _logger = Substitute.For<ILogger<CsvToExcelService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly CsvToExcelService _sut;

    public CsvToExcelServiceTests()
    {
        _sut = new CsvToExcelService(_logger, _fetchService);
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
        var request = new CsvToExcelRequest("");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidCsvWithWrongSeparatorOption_MayFailOrSucceed()
    {
        var base64 = CreateTestCsvBase64Internal(new[] { new Dictionary<string, object> { ["A"] = "1" } });
        var request = new CsvToExcelRequest(base64, Separator: ";");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidCsv_WithoutOptions_ReturnsExcelBase64()
    {
        var base64 = CreateTestCsvBase64Internal(new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 },
            new Dictionary<string, object> { ["Nombre"] = "Bob", ["Edad"] = 25 }
        });
        var request = new CsvToExcelRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value!.Base64Content));

        var excelBytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(excelBytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_ValidCsv_WithSeparatorSemicolon_ReturnsExcel()
    {
        var base64 = CreateTestCsvBase64WithSeparatorInternal(';', new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        var request = new CsvToExcelRequest(base64, Separator: ";");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        var excelBytes = Convert.FromBase64String(result.Value!.Base64Content);
        Assert.True(excelBytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_InvalidEncoding_ReturnsFailure()
    {
        var base64 = CreateTestCsvBase64Internal(new[] { new Dictionary<string, object> { ["A"] = 1 } });
        var request = new CsvToExcelRequest(base64, Encoding: "invalid-encoding-xyz");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlSuccess_ReturnsExcel()
    {
        var csvBytes = CreateTestCsvBytesInternal(new[] { new Dictionary<string, object> { ["A"] = "1" } });
        _fetchService.FetchAsync("https://example.com/file.csv")
            .Returns(Result<byte[]>.Success(csvBytes));

        var request = new CsvToExcelRequest(null, DocumentUrl: "https://example.com/file.csv");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var request = new CsvToExcelRequest(null, DocumentUrl: "https://example.com/notfound.csv");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    private static string CreateTestCsvBase64Internal(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows, excelType: ExcelType.CSV);
        return Convert.ToBase64String(stream.ToArray());
    }

    private static byte[] CreateTestCsvBytesInternal(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows, excelType: ExcelType.CSV);
        return stream.ToArray();
    }

    private static string CreateTestCsvBase64WithSeparatorInternal(char separator, IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        var config = new MiniExcelLibs.Csv.CsvConfiguration { Seperator = separator };
        stream.SaveAs(rows, excelType: ExcelType.CSV, configuration: config);
        return Convert.ToBase64String(stream.ToArray());
    }
}
