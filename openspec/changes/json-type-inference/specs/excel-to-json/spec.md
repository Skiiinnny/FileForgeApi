## ADDED Requirements

### Requirement: Response rows always use JsonElement values
The endpoint SHALL return rows as `List<Dictionary<string, JsonElement>>`. When `inferTypes` is `false` (default), every `JsonElement` wraps a string, preserving prior behavior. When `inferTypes` is `true`, numeric and boolean cell values are emitted as their native JSON types.

#### Scenario: Default response emits all values as JSON strings
- **WHEN** the request omits `inferTypes` (or sets it to `false`) and the Excel file contains numeric cell `42`
- **THEN** the response row contains `{ "col": "42" }` (JsonElement with ValueKind String)

#### Scenario: Type inference converts integer cell to JSON number
- **WHEN** `inferTypes` is `true` and an Excel cell contains the number `42`
- **THEN** the response row contains `{ "col": 42 }` (JsonElement with ValueKind Number)

#### Scenario: Type inference converts decimal cell to JSON number
- **WHEN** `inferTypes` is `true` and an Excel cell contains the number `3.14`
- **THEN** the response row contains `{ "col": 3.14 }` (JsonElement with ValueKind Number)

#### Scenario: Type inference converts boolean-like string "true" to JSON boolean
- **WHEN** `inferTypes` is `true` and a cell contains the text `true` (case-insensitive)
- **THEN** the response row contains `{ "col": true }` (JsonElement with ValueKind True)

#### Scenario: Type inference converts boolean-like string "false" to JSON boolean
- **WHEN** `inferTypes` is `true` and a cell contains the text `false` (case-insensitive)
- **THEN** the response row contains `{ "col": false }` (JsonElement with ValueKind False)

#### Scenario: Empty cell emits empty string regardless of inferTypes
- **WHEN** an Excel cell is empty
- **THEN** the response row contains `{ "col": "" }` regardless of `inferTypes`

#### Scenario: Non-numeric text string stays as string with inferTypes true
- **WHEN** `inferTypes` is `true` and a cell contains `"hello"`
- **THEN** the response row contains `{ "col": "hello" }` (JsonElement with ValueKind String)

### Requirement: inferTypes is an optional request body field
The request record SHALL expose `inferTypes` as an optional boolean property with a default of `false`. It MUST NOT be a query parameter.

#### Scenario: Request without inferTypes field is accepted
- **WHEN** the request body does not include the `inferTypes` field
- **THEN** the endpoint processes the file and returns all values as strings
