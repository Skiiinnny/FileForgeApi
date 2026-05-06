## MODIFIED Requirements

### Requirement: Response sheet rows always use JsonElement values
The endpoint SHALL return a map of type `Dictionary<string, PaginatedResponse<Dictionary<string, JsonElement>>>`. When `inferTypes` is `false` (default), all values are JSON strings. When `inferTypes` is `true`, numeric and boolean cell values are emitted as their native JSON types, applied uniformly across all sheets.

#### Scenario: Default response emits all values as strings across all sheets
- **WHEN** `inferTypes` is omitted and multiple sheets contain numeric data
- **THEN** every value in every `Items` collection for every sheet is a JSON string

#### Scenario: Type inference applies to all sheets equally
- **WHEN** `inferTypes` is `true` and both sheet "A" and sheet "B" contain numeric cells
- **THEN** numeric values in both sheets' `Items` are emitted as JSON numbers

#### Scenario: Sheet with no rows returns empty Items for that sheet
- **WHEN** a sheet in the Excel file is empty (header row only or completely empty)
- **THEN** the corresponding key in the response maps to a `PaginatedResponse` where `Items` is an empty array `[]`

## ADDED Requirements

### Requirement: Multi-Sheet Excel to JSON Pagination
The endpoint SHALL accept `Page` and `PageSize` optional parameters in the request body. If omitted, they MUST default to 1 and 100 respectively. These parameters SHALL apply uniformly to every sheet in the workbook.

#### Scenario: Request with specific page and page size for all sheets
- **WHEN** a request is made with `Page = 2` and `PageSize = 10`
- **THEN** the response SHALL contain the 11th through 20th rows for EVERY sheet in the workbook, and each sheet's metadata SHOULD reflect `Page = 2` and `PageSize = 10`
