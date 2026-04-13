using FileForgeApi.Features.ExcelToJsonMultiSheet;

namespace FileForgeApi.Tests.Features.ExcelToJsonMultiSheet;

public class ExcelToJsonMultiSheetValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = ExcelToJsonMultiSheetValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_EmptyBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonMultiSheetRequest("");

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validate_InvalidBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonMultiSheetRequest("not-valid-base64!!!");

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64 válida", result.Error!);
    }

    [Fact]
    public void Validate_ValidBase64_ReturnsSuccessWithBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToJsonMultiSheetRequest(Convert.ToBase64String(bytes));

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
        Assert.False(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var request = new ExcelToJsonMultiSheetRequest(Convert.ToBase64String(bytes), "https://example.com/f.xlsx");

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public void Validate_ValidDocumentUrl_ReturnsSuccessWithUseUrl()
    {
        var request = new ExcelToJsonMultiSheetRequest(null, "https://example.com/file.xlsx");

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_InvalidUrl_ReturnsFailure()
    {
        var request = new ExcelToJsonMultiSheetRequest(null, "not-a-url");

        var result = ExcelToJsonMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }
}
