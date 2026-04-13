using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Features.JsonToExcelMultiSheet;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FileForgeApi.Tests.Features.JsonToExcelMultiSheet;

public class JsonToExcelMultiSheetServiceTests
{
    private readonly ILogger<JsonToExcelMultiSheetService> _logger =
        Substitute.For<ILogger<JsonToExcelMultiSheetService>>();
    private readonly JsonToExcelMultiSheetService _sut;

    public JsonToExcelMultiSheetServiceTests()
    {
        _sut = new JsonToExcelMultiSheetService(_logger);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptySheets_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, string>>>());

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidSheets_ReturnsBase64Excel()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, string>>>
        {
            ["Sheet1"] =
            [
                new Dictionary<string, string> { ["Nombre"] = "Alice", ["Edad"] = "30" },
                new Dictionary<string, string> { ["Nombre"] = "Bob", ["Edad"] = "25" }
            ]
        });

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));

        var bytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_MultipleSheets_GeneratesExcelWithAllSheets()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, string>>>
        {
            ["Hoja1"] =
            [
                new Dictionary<string, string> { ["Col1"] = "A1", ["Col2"] = "B1" }
            ],
            ["Hoja2"] =
            [
                new Dictionary<string, string> { ["X"] = "x1", ["Y"] = "y1" }
            ]
        });

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var excelBytes = Convert.FromBase64String(result.Value!.Base64Content);
        var excelRequest = new ExcelToJsonMultiSheetRequest(Convert.ToBase64String(excelBytes));
        var excelService = new ExcelToJsonMultiSheetService(Substitute.For<ILogger<ExcelToJsonMultiSheetService>>(), Substitute.For<FileForgeApi.Shared.Documents.IDocumentFetchService>());
        var readResult = await excelService.ConvertAsync(excelRequest);

        Assert.True(readResult.IsSuccess);
        Assert.Equal(2, readResult.Value!.Sheets.Count);
        Assert.True(readResult.Value.Sheets.ContainsKey("Hoja1"));
        Assert.True(readResult.Value.Sheets.ContainsKey("Hoja2"));
        Assert.Equal("A1", readResult.Value.Sheets["Hoja1"][0]["Col1"]);
        Assert.Equal("x1", readResult.Value.Sheets["Hoja2"][0]["X"]);
    }
}
