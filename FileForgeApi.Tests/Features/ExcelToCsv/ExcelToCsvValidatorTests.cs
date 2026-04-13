using FileForgeApi.Features.ExcelToCsv;

namespace FileForgeApi.Tests.Features.ExcelToCsv;

public class ExcelToCsvValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = ExcelToCsvValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_EmptyBase64_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest("");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validate_InvalidBase64_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest("not-valid-base64!!!");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64 válida", result.Error!);
    }

    [Fact]
    public void Validate_EmptyFileAfterDecode_ReturnsFailure()
    {
        var emptyBase64Request = new ExcelToCsvRequest(Convert.ToBase64String(Array.Empty<byte>()));
        var result = ExcelToCsvValidator.Validate(emptyBase64Request);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Validate_ValidBase64_ReturnsSuccessWithBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToCsvRequest(Convert.ToBase64String(bytes));

        var result = ExcelToCsvValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
        Assert.False(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_SeparatorWithMultipleChars_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest(Convert.ToBase64String(new byte[] { 1 }), Separator: ";;");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Separator", result.Error!);
    }

    [Fact]
    public void Validate_InvalidEncoding_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest(Convert.ToBase64String(new byte[] { 1 }), Encoding: "invalid-encoding");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public void Validate_InvalidNewLine_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest(Convert.ToBase64String(new byte[] { 1 }), NewLine: "invalid");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("NewLine", result.Error!);
    }

    [Fact]
    public void Validate_BothFields_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest(Convert.ToBase64String(new byte[] { 1 }), DocumentUrl: "https://example.com/f.xlsx");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public void Validate_ValidDocumentUrl_ReturnsSuccessWithUseUrl()
    {
        var request = new ExcelToCsvRequest(null, DocumentUrl: "https://example.com/file.xlsx");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_InvalidUrl_ReturnsFailure()
    {
        var request = new ExcelToCsvRequest(null, DocumentUrl: "not-a-url");

        var result = ExcelToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }
}
