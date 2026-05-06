## MODIFIED Requirements

### Requirement: Response rows always use JsonElement values
The endpoint SHALL return rows as `PaginatedResponse<Dictionary<string, JsonElement>>`. When `inferTypes` is `false` (default), every `JsonElement` wraps a string, preserving prior behavior. When `inferTypes` is `true`, numeric and boolean cell values are emitted as their native JSON types.

#### Scenario: Default response emits all values as JSON strings
- **WHEN** the request omits `inferTypes` (or sets it to `false`) and the Excel file contains numeric cell `42`
- **THEN** the response `Items` contain `{ "col": "42" }` (JsonElement with ValueKind String)

#### Scenario: Type inference converts integer cell to JSON number
- **WHEN** `inferTypes` is `true` and an Excel cell contains the number `42`
- **THEN** the response `Items` contain `{ "col": 42 }` (JsonElement with ValueKind Number)

#### Scenario: Type inference converts decimal cell to JSON number
- **WHEN** `inferTypes` is `true` and an Excel cell contains the number `3.14`
- **THEN** the response `Items` contain `{ "col": 3.14 }` (JsonElement with ValueKind Number)

#### Scenario: Type inference converts boolean-like string "true" to JSON boolean
- **WHEN** `inferTypes` is `true` and a cell contains the text `true` (case-insensitive)
- **THEN** the response `Items` contain `{ "col": true }` (JsonElement with ValueKind True)

#### Scenario: Type inference converts boolean-like string "false" to JSON boolean
- **WHEN** `inferTypes` is `true` and a cell contains the text `false` (case-insensitive)
- **THEN** the response `Items` contain `{ "col": false }` (JsonElement with ValueKind False)

#### Scenario: Empty cell emits empty string regardless of inferTypes
- **WHEN** an Excel cell is empty
- **THEN** the response `Items` contain `{ "col": "" }` regardless of `inferTypes`

#### Scenario: Non-numeric text string stays as string with inferTypes true
- **WHEN** `inferTypes` is `true` and a cell contains `"hello"`
- **THEN** the response `Items` contain `{ "col": "hello" }` (JsonElement with ValueKind String)

## ADDED Requirements

### Requirement: Excel to JSON Pagination
The endpoint SHALL accept `Page` and `PageSize` optional parameters in the request body. If omitted, they MUST default to 1 and 100 respectively.

#### Scenario: Request with specific page and page size
- **WHEN** a request is made with `Page = 2` and `PageSize = 10`
- **THEN** the response SHALL contain the 11th through 20th rows of the Excel file, and metadata SHOULD reflect `Page = 2` and `PageSize = 10`
