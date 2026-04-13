using FileForgeApi.Features.CsvToJson;
using FileForgeApi.Features.JsonToCsv;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.JsonToCsv;

public class JsonToCsvServiceTests
{
    private readonly ILogger<JsonToCsvService> _logger = Substitute.For<ILogger<JsonToCsvService>>();
    private readonly JsonToCsvService _sut;

    public JsonToCsvServiceTests()
    {
        _sut = new JsonToCsvService(_logger);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptyRows_ReturnsFailure()
    {
        var request = new JsonToCsvRequest([]);

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidRows_ReturnsBase64Csv()
    {
        var request = new JsonToCsvRequest(
        [
            new Dictionary<string, string> { ["Nombre"] = "Alice", ["Edad"] = "30" },
            new Dictionary<string, string> { ["Nombre"] = "Bob", ["Edad"] = "25" }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));

        var bytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_ValidRows_WithSeparatorSemicolon_ReturnsCsvWithSemicolon()
    {
        var request = new JsonToCsvRequest(
            [
                new Dictionary<string, string> { ["A"] = "1", ["B"] = "2" }
            ],
            Separator: ";");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        var bytes = Convert.FromBase64String(result.Value!.Base64Content);
        var csv = System.Text.Encoding.UTF8.GetString(bytes);
        Assert.Contains(";", csv);
    }

    [Fact]
    public async Task ConvertAsync_InvalidEncoding_ReturnsFailure()
    {
        var request = new JsonToCsvRequest(
            [new Dictionary<string, string> { ["A"] = "1" }],
            Encoding: "invalid-encoding-xyz");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_RoundTrip_JsonToCsvToJson_PreservesData()
    {
        var originalRows = new List<Dictionary<string, string>>
        {
            new() { ["Nombre"] = "Alice", ["Edad"] = "30" },
            new() { ["Nombre"] = "Bob", ["Edad"] = "25" }
        };
        var request = new JsonToCsvRequest(originalRows);

        var toCsvResult = await _sut.ConvertAsync(request);
        Assert.True(toCsvResult.IsSuccess);

        var csvBytes = Convert.FromBase64String(toCsvResult.Value!.Base64Content);
        var csvToJsonRequest = new CsvToJsonRequest(Convert.ToBase64String(csvBytes));
        var csvToJsonService = new CsvToJsonService(Substitute.For<ILogger<CsvToJsonService>>(), Substitute.For<FileForgeApi.Shared.Documents.IDocumentFetchService>());
        var fromCsvResult = await csvToJsonService.ConvertAsync(csvToJsonRequest);
        Assert.True(fromCsvResult.IsSuccess);

        var readBack = fromCsvResult.Value!.Rows;
        Assert.Equal(originalRows.Count, readBack.Count);
        Assert.Equal("Alice", readBack[0]["Nombre"]);
        Assert.Equal("30", readBack[0]["Edad"]);
        Assert.Equal("Bob", readBack[1]["Nombre"]);
        Assert.Equal("25", readBack[1]["Edad"]);
    }
}
