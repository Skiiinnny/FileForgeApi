## Why

Currently, endpoints that convert files to JSON (Excel to JSON, CSV to JSON) return the entire set of rows in a single response. For large files, this can lead to memory exhaustion in the AWS Lambda environment and slow down client-side processing. Implementing a standard pagination mechanism ensures the API remains performant and scalable.

## What Changes

- **BREAKING**: The response structure for `ExcelToJson`, `CsvToJson`, and `ExcelToJsonMultiSheet` will change from a simple list/dictionary of rows to a paginated object.
- **New Parameters**: `ExcelToJsonRequest`, `CsvToJsonRequest`, and `ExcelToJsonMultiSheetRequest` will now include optional `Page` and `PageSize` fields.
- **Shared Models**: Introduction of `PaginationParams` and `PaginatedResponse<T>` in a new `Shared/Pagination` namespace.
- **Source Generation**: Registration of all new paginated response types in `AppJsonSerializerContext` to maintain AOT compatibility.

## Capabilities

### New Capabilities
- `pagination-core`: Shared infrastructure providing base records for pagination requests and responses.

### Modified Capabilities
- `excel-to-json`: Updated to accept pagination parameters and return a paginated list of rows.
- `csv-to-json`: Updated to accept pagination parameters and return a paginated list of rows.
- `excel-to-json-multi-sheet`: Updated to support pagination per sheet.

## Impact

- **Public API**: Affects `POST /api/excel/to-json`, `POST /api/csv/to-json`, and `POST /api/excel/to-json-multi-sheet`.
- **Serialization**: Requires updates to `Shared/Serialization/AppJsonSerializerContext.cs`.
- **Documentation**: README.md needs to be updated to reflect the new response schemas.
