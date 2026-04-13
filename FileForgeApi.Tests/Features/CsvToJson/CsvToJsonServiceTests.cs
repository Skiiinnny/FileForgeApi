using FileForgeApi.Features.CsvToJson;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.CsvToJson;

public class CsvToJsonServiceTests
{
    private readonly ILogger<CsvToJsonService> _logger = Substitute.For<ILogger<CsvToJsonService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly CsvToJsonService _sut;

    public CsvToJsonServiceTests()
    {
        _sut = new CsvToJsonService(_logger, _fetchService);
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
        var request = new CsvToJsonRequest("");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_InvalidCsvContent_ReturnsFailure()
    {
        var excelBase64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["A"] = 1 }
        });
        var request = new CsvToJsonRequest(excelBase64);

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo leer el CSV", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_ValidCsv_ReturnsRows()
    {
        var base64 = CreateTestCsvBase64Internal(new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 },
            new Dictionary<string, object> { ["Nombre"] = "Bob", ["Edad"] = 25 }
        });
        var request = new CsvToJsonRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Rows.Count);
        Assert.Equal("Alice", result.Value.Rows[0]["Nombre"]);
        Assert.Equal("30", result.Value.Rows[0]["Edad"]);
        Assert.Equal("Bob", result.Value.Rows[1]["Nombre"]);
        Assert.Equal("25", result.Value.Rows[1]["Edad"]);
    }

    [Fact]
    public async Task ConvertAsync_EmptyCsv_ReturnsEmptyList()
    {
        var base64 = CreateTestCsvBase64Internal(Array.Empty<Dictionary<string, object>>());
        var request = new CsvToJsonRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!.Rows);
    }

    [Fact]
    public async Task ConvertAsync_ValidCsv_WithSeparatorSemicolon_ReturnsRows()
    {
        var base64 = CreateTestCsvBase64WithSeparator(';', new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        var request = new CsvToJsonRequest(base64, Separator: ";");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Rows);
        Assert.Equal("1", result.Value.Rows[0]["A"]);
        Assert.Equal("2", result.Value.Rows[0]["B"]);
    }

    [Fact]
    public async Task ConvertAsync_InvalidEncoding_ReturnsFailure()
    {
        var base64 = CreateTestCsvBase64Internal(new[] { new Dictionary<string, object> { ["A"] = 1 } });
        var request = new CsvToJsonRequest(base64, Encoding: "invalid-encoding-xyz");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlSuccess_ReturnsRows()
    {
        var csvBytes = CreateTestCsvBytesInternal(new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        _fetchService.FetchAsync("https://example.com/data.csv")
            .Returns(Result<byte[]>.Success(csvBytes));

        var request = new CsvToJsonRequest(null, DocumentUrl: "https://example.com/data.csv");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var request = new CsvToJsonRequest(null, DocumentUrl: "https://example.com/notfound.csv");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    private static string CreateTestExcelBase64(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows, excelType: ExcelType.XLSX);
        return Convert.ToBase64String(stream.ToArray());
    }

    private static string CreateTestCsvBase64WithSeparator(char separator, IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        var config = new MiniExcelLibs.Csv.CsvConfiguration { Seperator = separator };
        stream.SaveAs(rows, excelType: ExcelType.CSV, configuration: config);
        return Convert.ToBase64String(stream.ToArray());
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
}
