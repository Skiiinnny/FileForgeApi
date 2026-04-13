using System.Text.Json.Serialization;
using FileForgeApi.Features.Base64ToCsv;
using FileForgeApi.Features.Base64ToExcel;
using FileForgeApi.Features.Base64ToJson;
using FileForgeApi.Features.CsvToExcel;
using FileForgeApi.Features.CsvToExcelMultiSheet;
using FileForgeApi.Features.CsvToJson;
using FileForgeApi.Features.ExcelAppendRows;
using FileForgeApi.Features.ExcelFromTemplate;
using FileForgeApi.Features.ExcelMetadata;
using FileForgeApi.Features.ExcelToCsv;
using FileForgeApi.Features.ExcelToJson;
using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Features.JsonToCsv;
using FileForgeApi.Features.JsonToExcel;
using FileForgeApi.Features.JsonToExcelMultiSheet;

namespace FileForgeApi.Shared.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Base64ToCsvRequest))]
[JsonSerializable(typeof(Base64ToExcelRequest))]
[JsonSerializable(typeof(Base64ToJsonRequest))]
[JsonSerializable(typeof(CsvToExcelRequest))]
[JsonSerializable(typeof(CsvToExcelResponse))]
[JsonSerializable(typeof(CsvToExcelMultiSheetRequest))]
[JsonSerializable(typeof(CsvToExcelMultiSheetResponse))]
[JsonSerializable(typeof(CsvToJsonRequest))]
[JsonSerializable(typeof(CsvToJsonResponse))]
[JsonSerializable(typeof(ExcelAppendRowsRequest))]
[JsonSerializable(typeof(ExcelAppendRowsResponse))]
[JsonSerializable(typeof(ExcelFromTemplateRequest))]
[JsonSerializable(typeof(ExcelFromTemplateResponse))]
[JsonSerializable(typeof(ExcelMetadataRequest))]
[JsonSerializable(typeof(ExcelMetadataResponse))]
[JsonSerializable(typeof(ExcelToCsvRequest))]
[JsonSerializable(typeof(ExcelToCsvResponse))]
[JsonSerializable(typeof(ExcelToJsonRequest))]
[JsonSerializable(typeof(ExcelToJsonResponse))]
[JsonSerializable(typeof(ExcelToJsonMultiSheetRequest))]
[JsonSerializable(typeof(ExcelToJsonMultiSheetResponse))]
[JsonSerializable(typeof(JsonToCsvRequest))]
[JsonSerializable(typeof(JsonToCsvResponse))]
[JsonSerializable(typeof(JsonToExcelRequest))]
[JsonSerializable(typeof(JsonToExcelResponse))]
[JsonSerializable(typeof(JsonToExcelMultiSheetRequest))]
[JsonSerializable(typeof(JsonToExcelMultiSheetResponse))]
[JsonSerializable(typeof(List<Dictionary<string, string>>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, List<Dictionary<string, string>>>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest))]
[JsonSerializable(typeof(Amazon.Lambda.APIGatewayEvents.APIGatewayProxyResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext;
