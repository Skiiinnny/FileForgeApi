using System.Text.Json;
using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.ExcelToJsonMultiSheet;

public class ExcelToJsonMultiSheetServiceTests
{
    private readonly ILogger<ExcelToJsonMultiSheetService> _logger =
        Substitute.For<ILogger<ExcelToJsonMultiSheetService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelToJsonMultiSheetService _sut;

    public ExcelToJsonMultiSheetServiceTests()
    {
        _sut = new ExcelToJsonMultiSheetService(_logger, _fetchService);
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
        var request = new ExcelToJsonMultiSheetRequest("");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_InvalidExcelContent_ReturnsFailure()
    {
        var invalidBytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToJsonMultiSheetRequest(Convert.ToBase64String(invalidBytes));

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo leer el Excel", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_InferTypesFalse_ReturnsStringElements()
    {
        var base64 = CreateTestMultiSheetExcelBase64(new Dictionary<string, object>
        {
            ["Hoja1"] = new[]
            {
                new Dictionary<string, object> { ["Col1"] = "A1", ["Col2"] = 1 },
                new Dictionary<string, object> { ["Col1"] = "A2", ["Col2"] = 2 }
            },
            ["Hoja2"] = new[]
            {
                new Dictionary<string, object> { ["X"] = "x1", ["Y"] = "y1" }
            }
        });
        var request = new ExcelToJsonMultiSheetRequest(base64, InferTypes: false);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Sheets.Count);
        Assert.Equal(JsonValueKind.String, result.Value.Sheets["Hoja1"][0]["Col2"].ValueKind);
        Assert.Equal("1", result.Value.Sheets["Hoja1"][0]["Col2"].GetString());
        Assert.Equal("x1", result.Value.Sheets["Hoja2"][0]["X"].GetString());
    }

    [Fact]
    public async Task ConvertAsync_InferTypesTrue_ReturnsTypedElements()
    {
        var base64 = CreateTestMultiSheetExcelBase64(new Dictionary<string, object>
        {
            ["Hoja1"] = new[]
            {
                new Dictionary<string, object> { ["Col1"] = "texto", ["Col2"] = 42 }
            }
        });
        var request = new ExcelToJsonMultiSheetRequest(base64, InferTypes: true);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(JsonValueKind.String, result.Value!.Sheets["Hoja1"][0]["Col1"].ValueKind);
        Assert.Equal(JsonValueKind.Number, result.Value.Sheets["Hoja1"][0]["Col2"].ValueKind);
        Assert.Equal(42, result.Value.Sheets["Hoja1"][0]["Col2"].GetInt32());
    }

    [Fact]
    public async Task ConvertAsync_SingleSheetExcel_ReturnsOneSheet()
    {
        var base64 = CreateTestMultiSheetExcelBase64(new Dictionary<string, object>
        {
            ["Sheet1"] = new[]
            {
                new Dictionary<string, object> { ["A"] = "valor" }
            }
        });
        var request = new ExcelToJsonMultiSheetRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Sheets);
        Assert.True(result.Value.Sheets.ContainsKey("Sheet1"));
        Assert.Single(result.Value.Sheets["Sheet1"]);
        Assert.Equal("valor", result.Value.Sheets["Sheet1"][0]["A"].GetString());
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlSuccess_ReturnsSheets()
    {
        var excelBytes = CreateTestMultiSheetExcelBytes(new Dictionary<string, object>
        {
            ["Sheet1"] = new[] { new Dictionary<string, object> { ["A"] = "1" } }
        });
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(excelBytes));

        var request = new ExcelToJsonMultiSheetRequest(null, "https://example.com/file.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var request = new ExcelToJsonMultiSheetRequest(null, "https://example.com/notfound.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    private static string CreateTestMultiSheetExcelBase64(Dictionary<string, object> sheets)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        try
        {
            MiniExcel.SaveAs(tempPath, sheets);
            var bytes = File.ReadAllBytes(tempPath);
            return Convert.ToBase64String(bytes);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static byte[] CreateTestMultiSheetExcelBytes(Dictionary<string, object> sheets)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        try
        {
            MiniExcel.SaveAs(tempPath, sheets);
            return File.ReadAllBytes(tempPath);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
