## 1. Shared Infrastructure

- [x] 1.1 Create `Shared/Pagination/PaginationParams.cs` with `Page` and `PageSize`.
- [x] 1.2 Create `Shared/Pagination/PaginatedResponse.cs` with metadata and items.
- [x] 1.3 Update `Shared/Serialization/AppJsonSerializerContext.cs` to register:
    - `PaginatedResponse<Dictionary<string, JsonElement>>`
    - `Dictionary<string, PaginatedResponse<Dictionary<string, JsonElement>>>`

## 2. Excel to JSON Pagination

- [x] 2.1 Update `ExcelToJsonRequest.cs`: add `int? Page` and `int? PageSize`.
- [x] 2.2 Update `ExcelToJsonResponse.cs`: change `Rows` to `PaginatedResponse<Dictionary<string, JsonElement>>`.
- [x] 2.3 Update `ExcelToJsonService.cs`: implement logic to skip/take rows and build `PaginatedResponse`.
- [x] 2.4 Update `ExcelToJsonValidator.cs` if necessary (e.g., validate `PageSize > 0`).
- [x] 2.5 Update `ExcelToJsonEndpoint.cs` to ensure it still maps correctly with the new response type.
- [x] 2.6 Update/Add tests in `FileForgeApi.Tests/Features/ExcelToJson/` to verify pagination.

## 3. CSV to JSON Pagination

- [x] 3.1 Update `CsvToJsonRequest.cs`: add `int? Page` and `int? PageSize`.
- [x] 3.2 Update `CsvToJsonResponse.cs`: change `Rows` to `PaginatedResponse<Dictionary<string, JsonElement>>`.
- [x] 3.3 Update `CsvToJsonService.cs`: implement logic to skip/take rows and build `PaginatedResponse`.
- [x] 3.4 Update `CsvToJsonValidator.cs` if necessary.
- [x] 3.5 Update `CsvToJsonEndpoint.cs`.
- [x] 3.6 Update/Add tests in `FileForgeApi.Tests/Features/CsvToJson/` to verify pagination.

## 4. Multi-Sheet Excel to JSON Pagination

- [x] 4.1 Update `ExcelToJsonMultiSheetRequest.cs`: add `int? Page` and `int? PageSize`.
- [x] 4.2 Update `ExcelToJsonMultiSheetResponse.cs`: change `Sheets` to `Dictionary<string, PaginatedResponse<Dictionary<string, JsonElement>>>`.
- [x] 4.3 Update `ExcelToJsonMultiSheetService.cs`: implement pagination logic per sheet.
- [x] 4.4 Update `ExcelToJsonMultiSheetValidator.cs` if necessary.
- [x] 4.5 Update `ExcelToJsonMultiSheetEndpoint.cs`.
- [x] 4.6 Update/Add tests in `FileForgeApi.Tests/Features/ExcelToJsonMultiSheet/` to verify pagination.

## 5. Documentation

- [x] 5.1 Update `Readme.md` to document the new pagination parameters and response structure.
