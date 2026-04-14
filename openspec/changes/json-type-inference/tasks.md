## 1. Shared Infrastructure

- [x] 1.1 Create `Shared/Json/JsonTypeInferenceHelper.cs` — static `TryInfer(string raw, out JsonElement element)` that attempts null → bool → long → double → string fallback using InvariantCulture; return `JsonElement` via `JsonDocument.Parse(...).RootElement.Clone()`
- [x] 1.2 Create `Shared/Json/JsonElementExtensions.cs` — static `ToClrObject(this JsonElement el)` that maps ValueKind to CLR types: String→string, Number→double, True→true, False→false, Null→null
- [x] 1.3 Register new shared types in `Shared/Serialization/AppJsonSerializerContext.cs`: add `[JsonSerializable(typeof(Dictionary<string, JsonElement>))]`, `[JsonSerializable(typeof(List<Dictionary<string, JsonElement>>))]`, and `[JsonSerializable(typeof(Dictionary<string, List<Dictionary<string, JsonElement>>>))]`

## 2. json-to-csv — Accept Typed Input

- [x] 2.1 Update `Features/JsonToCsv/JsonToCsvRequest.cs` — change `List<Dictionary<string, string>> Rows` to `List<Dictionary<string, JsonElement>> Rows`
- [x] 2.2 Update `Features/JsonToCsv/JsonToCsvService.cs` — convert each `JsonElement` to CLR object via `ToClrObject()` before passing rows to MiniExcel; string fallback writes value using `InvariantCulture`
- [x] 2.3 Update `Features/JsonToCsv/JsonToCsvValidator.cs` — adjust validation to check `JsonElement` row list instead of string list (null/empty checks remain the same)

## 3. json-to-excel — Accept Typed Input

- [x] 3.1 Update `Features/JsonToExcel/JsonToExcelRequest.cs` — change `List<Dictionary<string, string>> Rows` to `List<Dictionary<string, JsonElement>> Rows`
- [x] 3.2 Update `Features/JsonToExcel/JsonToExcelService.cs` — use `ToClrObject()` to extract native CLR values before calling `stream.SaveAsAsync(rows)`
- [x] 3.3 Update `Features/JsonToExcel/JsonToExcelValidator.cs` — adjust null/empty checks for `JsonElement` row list

## 4. json-to-excel-multi-sheet — Accept Typed Input

- [x] 4.1 Update `Features/JsonToExcelMultiSheet/JsonToExcelMultiSheetRequest.cs` — change `Dictionary<string, List<Dictionary<string, string>>>` to `Dictionary<string, List<Dictionary<string, JsonElement>>>`
- [x] 4.2 Update `Features/JsonToExcelMultiSheet/JsonToExcelMultiSheetService.cs` — apply `ToClrObject()` per cell before each sheet's `SaveAsAsync` call
- [x] 4.3 Update `Features/JsonToExcelMultiSheet/JsonToExcelMultiSheetValidator.cs` — adjust null/empty checks for updated sheet type

## 5. excel-to-json — Add inferTypes and Typed Output

- [x] 5.1 Update `Features/ExcelToJson/ExcelToJsonRequest.cs` — add optional `bool? InferTypes = false` property
- [x] 5.2 Update `Features/ExcelToJson/ExcelToJsonResponse.cs` — change `List<Dictionary<string, string>> Rows` to `List<Dictionary<string, JsonElement>> Rows`
- [x] 5.3 Update `Features/ExcelToJson/IExcelToJsonService.cs` — adjust return type signature if needed
- [x] 5.4 Update `Features/ExcelToJson/ExcelToJsonService.cs` — when building the output row dict, call `JsonTypeInferenceHelper.TryInfer()` if `request.InferTypes == true`, otherwise wrap the raw string in a `JsonElement` string value
- [x] 5.5 Update `Features/ExcelToJson/ExcelToJsonService.Logging.cs` — ensure logging partial still compiles with updated types

## 6. excel-to-json-multi-sheet — Add inferTypes and Typed Output

- [x] 6.1 Update `Features/ExcelToJsonMultiSheet/ExcelToJsonMultiSheetRequest.cs` — add optional `bool? InferTypes = false` property
- [x] 6.2 Update `Features/ExcelToJsonMultiSheet/ExcelToJsonMultiSheetResponse.cs` — change sheet rows to `Dictionary<string, List<Dictionary<string, JsonElement>>>`
- [x] 6.3 Update `Features/ExcelToJsonMultiSheet/IExcelToJsonMultiSheetService.cs` — adjust return type signature if needed
- [x] 6.4 Update `Features/ExcelToJsonMultiSheet/ExcelToJsonMultiSheetService.cs` — apply inference logic per-cell per-sheet, controlled by `request.InferTypes`
- [x] 6.5 Update `Features/ExcelToJsonMultiSheet/ExcelToJsonMultiSheetService.Logging.cs` — ensure logging partial compiles

## 7. csv-to-json — Add inferTypes and Typed Output

- [x] 7.1 Update `Features/CsvToJson/CsvToJsonRequest.cs` — add optional `bool? InferTypes = false` property
- [x] 7.2 Update `Features/CsvToJson/CsvToJsonResponse.cs` — change `List<Dictionary<string, string>> Rows` to `List<Dictionary<string, JsonElement>> Rows`
- [x] 7.3 Update `Features/CsvToJson/ICsvToJsonService.cs` — adjust return type signature if needed
- [x] 7.4 Update `Features/CsvToJson/CsvToJsonService.cs` — apply inference logic per-cell, controlled by `request.InferTypes`
- [x] 7.5 Update `Features/CsvToJson/CsvToJsonService.Logging.cs` — ensure logging partial compiles

## 8. Tests

- [x] 8.1 Update `FileForgeApi.Tests/Features/JsonToCsv/` — update existing string-row tests to use `JsonElement` rows; add test cases for numeric, boolean, and null values
- [x] 8.2 Update `FileForgeApi.Tests/Features/JsonToExcel/` — add typed-value test cases; verify numeric cells are stored as numbers in Excel output
- [x] 8.3 Update `FileForgeApi.Tests/Features/JsonToExcelMultiSheet/` — add typed-value test cases across multiple sheets
- [x] 8.4 Update `FileForgeApi.Tests/Features/ExcelToJson/` — add tests for `inferTypes: false` (default) and `inferTypes: true` with numeric, boolean, and text cells
- [x] 8.5 Update `FileForgeApi.Tests/Features/ExcelToJsonMultiSheet/` — add `inferTypes` test cases across multiple sheets
- [x] 8.6 Update `FileForgeApi.Tests/Features/CsvToJson/` — add tests for `inferTypes: false` (default) and `inferTypes: true` for various value types
