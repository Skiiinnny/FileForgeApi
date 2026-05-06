## MODIFIED Requirements

### Requirement: Response rows always use JsonElement values
The endpoint SHALL return rows as `PaginatedResponse<Dictionary<string, JsonElement>>`. When `inferTypes` is `false` (default), every `JsonElement` wraps a string, preserving prior behavior. When `inferTypes` is `true`, values that parse as numbers or booleans are emitted as their native JSON types.

#### Scenario: Default response emits all CSV values as JSON strings
- **WHEN** `inferTypes` is omitted and the CSV contains `name,age\nAlice,30`
- **THEN** the response `Items` contain `[{ "name": "Alice", "age": "30" }]` (both as strings)

#### Scenario: Type inference converts integer string to JSON number
- **WHEN** `inferTypes` is `true` and a CSV cell contains `42`
- **THEN** the response `Items` emit `42` as a JSON number (not `"42"`)

#### Scenario: Type inference converts decimal string to JSON number
- **WHEN** `inferTypes` is `true` and a CSV cell contains `3.14`
- **THEN** the response `Items` emit `3.14` as a JSON number using invariant-culture parsing

#### Scenario: Type inference converts "true"/"false" strings to JSON booleans
- **WHEN** `inferTypes` is `true` and a CSV cell contains `true` or `false` (case-insensitive)
- **THEN** the response `Items` emit the value as JSON boolean `true` or `false`

#### Scenario: Non-numeric strings remain strings with inferTypes true
- **WHEN** `inferTypes` is `true` and a CSV cell contains `hello`
- **THEN** the response `Items` emit `"hello"` as a JSON string

#### Scenario: Empty CSV cell emits empty string regardless of inferTypes
- **WHEN** a CSV cell is empty
- **THEN** the response `Items` emit `""` for that cell regardless of `inferTypes`

## ADDED Requirements

### Requirement: CSV to JSON Pagination
The endpoint SHALL accept `Page` and `PageSize` optional parameters in the request body. If omitted, they MUST default to 1 and 100 respectively.

#### Scenario: Request with specific page and page size
- **WHEN** a request is made with `Page = 2` and `PageSize = 10`
- **THEN** the response SHALL contain the 11th through 20th rows of the CSV file, and metadata SHOULD reflect `Page = 2` and `PageSize = 10`
