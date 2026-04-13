using FileForgeApi.Features.CsvToExcelMultiSheet;
using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using MiniExcelLibs.Csv;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace FileForgeApi.Tests.Features.CsvToExcelMultiSheet;

public class CsvToExcelMultiSheetTests : IAsyncDisposable
{
    private readonly ILogger<CsvToExcelMultiSheetService> _logger =
        Substitute.For<ILogger<CsvToExcelMultiSheetService>>();
    private readonly CsvToExcelMultiSheetService _service;
    private readonly ILogger<ExcelToJsonMultiSheetService> _excelLogger =
        Substitute.For<ILogger<ExcelToJsonMultiSheetService>>();
    private readonly ExcelToJsonMultiSheetService _excelService;

    private readonly ICsvToExcelMultiSheetService _mockService =
        Substitute.For<ICsvToExcelMultiSheetService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public CsvToExcelMultiSheetTests()
    {
        _service = new CsvToExcelMultiSheetService(_logger);
        _excelService = new ExcelToJsonMultiSheetService(_excelLogger, Substitute.For<FileForgeApi.Shared.Documents.IDocumentFetchService>());

        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(_mockService);
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapCsvToExcelMultiSheet());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    // 6.2 - Successful conversion with two distinct sheets
    [Fact]
    public async Task ConvertAsync_TwoSheets_ReturnsExcelWithBothSheets()
    {
        var ventasCsv = CreateCsvBase64(',', new[]
        {
            new Dictionary<string, object> { ["Producto"] = "Manzana", ["Cantidad"] = 10 },
            new Dictionary<string, object> { ["Producto"] = "Pera", ["Cantidad"] = 5 }
        });
        var gastosCsv = CreateCsvBase64(',', new[]
        {
            new Dictionary<string, object> { ["Concepto"] = "Alquiler", ["Monto"] = 1000 }
        });

        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>
        {
            ["Ventas"] = ventasCsv,
            ["Gastos"] = gastosCsv
        }, Separator: ",", Encoding: "utf-8");

        var result = await _service.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.FileBase64));

        var excelRequest = new ExcelToJsonMultiSheetRequest(result.Value.FileBase64);
        var readResult = await _excelService.ConvertAsync(excelRequest);

        Assert.True(readResult.IsSuccess);
        Assert.Equal(2, readResult.Value!.Sheets.Count);
        Assert.True(readResult.Value.Sheets.ContainsKey("Ventas"));
        Assert.True(readResult.Value.Sheets.ContainsKey("Gastos"));
        Assert.Equal("Manzana", readResult.Value.Sheets["Ventas"][0]["Producto"].GetString());
        Assert.Equal("Alquiler", readResult.Value.Sheets["Gastos"][0]["Concepto"].GetString());
    }

    // 6.3 - Custom separator (;)
    [Fact]
    public async Task ConvertAsync_CustomSeparatorSemicolon_ParsesCorrectly()
    {
        var csvBase64 = CreateCsvBase64(';', new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 },
            new Dictionary<string, object> { ["Nombre"] = "Bob", ["Edad"] = 25 }
        });

        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>
        {
            ["Datos"] = csvBase64
        }, Separator: ";", Encoding: "utf-8");

        var result = await _service.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var excelRequest = new ExcelToJsonMultiSheetRequest(result.Value!.FileBase64);
        var readResult = await _excelService.ConvertAsync(excelRequest);

        Assert.True(readResult.IsSuccess);
        Assert.True(readResult.Value!.Sheets.ContainsKey("Datos"));
        Assert.Equal("Alice", readResult.Value.Sheets["Datos"][0]["Nombre"].GetString());
        Assert.Equal("30", readResult.Value.Sheets["Datos"][0]["Edad"].GetString());
    }

    // 6.4 - Empty Sheets → HTTP 400
    [Fact]
    public async Task ConvertAsync_EmptySheets_ReturnsFailure()
    {
        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>());

        var result = await _service.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Sheets", result.Error!);
    }

    [Fact]
    public async Task Endpoint_EmptySheets_ReturnsBadRequest()
    {
        _mockService
            .ConvertAsync(Arg.Any<CsvToExcelMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<CsvToExcelMultiSheetResponse>.Failure("Sheets vacío")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-excel/multi-sheet",
            new CsvToExcelMultiSheetRequest(new Dictionary<string, string>()));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    // 6.5 - Sheet name longer than 31 characters → HTTP 400
    [Fact]
    public async Task ConvertAsync_SheetNameTooLong_ReturnsFailure()
    {
        var longName = new string('A', 32);
        var csvBase64 = CreateCsvBase64(',', new[] { new Dictionary<string, object> { ["Col"] = "val" } });

        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>
        {
            [longName] = csvBase64
        });

        var result = await _service.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("31", result.Error!);
    }

    [Fact]
    public async Task Endpoint_SheetNameTooLong_ReturnsBadRequest()
    {
        _mockService
            .ConvertAsync(Arg.Any<CsvToExcelMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<CsvToExcelMultiSheetResponse>.Failure("Nombre de hoja supera 31 caracteres")));

        var longName = new string('A', 32);
        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-excel/multi-sheet",
            new CsvToExcelMultiSheetRequest(new Dictionary<string, string> { [longName] = "dGVzdA==" }));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    // 6.6 - Empty sheet name → HTTP 400
    [Fact]
    public async Task ConvertAsync_EmptySheetName_ReturnsFailure()
    {
        var csvBase64 = CreateCsvBase64(',', new[] { new Dictionary<string, object> { ["Col"] = "val" } });

        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>
        {
            [""] = csvBase64
        });

        var result = await _service.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("vacío", result.Error!);
    }

    [Fact]
    public async Task Endpoint_EmptySheetName_ReturnsBadRequest()
    {
        _mockService
            .ConvertAsync(Arg.Any<CsvToExcelMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<CsvToExcelMultiSheetResponse>.Failure("Nombre de hoja vacío")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-excel/multi-sheet",
            new CsvToExcelMultiSheetRequest(new Dictionary<string, string> { [""] = "dGVzdA==" }));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    // 6.7 - CSV with only headers (no data rows) → HTTP 200, sheet with headers and zero rows
    [Fact]
    public async Task ConvertAsync_CsvWithOnlyHeaders_ReturnsSuccessWithEmptyDataRows()
    {
        // Build a CSV with only a header row and no data rows
        var csvContent = "Nombre,Edad\r\n";
        var csvBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvContent));

        var request = new CsvToExcelMultiSheetRequest(new Dictionary<string, string>
        {
            ["Personas"] = csvBase64
        });

        var result = await _service.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var excelRequest = new ExcelToJsonMultiSheetRequest(result.Value!.FileBase64);
        var readResult = await _excelService.ConvertAsync(excelRequest);

        Assert.True(readResult.IsSuccess);
        Assert.True(readResult.Value!.Sheets.ContainsKey("Personas"));
        Assert.Empty(readResult.Value.Sheets["Personas"]);
    }

    private static string CreateCsvBase64(char separator, IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        var config = new CsvConfiguration { Seperator = separator };
        stream.SaveAs(rows, excelType: ExcelType.CSV, configuration: config);
        return Convert.ToBase64String(stream.ToArray());
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
