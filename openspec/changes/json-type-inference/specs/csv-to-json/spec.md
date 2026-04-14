## ADDED Requirements

### Requirement: Response rows always use JsonElement values
The endpoint SHALL return rows as `List<Dictionary<string, JsonElement>>`. When `inferTypes` is `false` (default), every `JsonElement` wraps a string, preserving prior behavior. When `inferTypes` is `true`, values that parse as numbers or booleans are emitted as their native JSON types.

#### Scenario: Default response emits all CSV values as JSON strings
- **WHEN** `inferTypes` is omitted and the CSV contains `name,age\nAlice,30`
- **THEN** the response contains `[{ "name": "Alice", "age": "30" }]` (both as strings)

#### Scenario: Type inference converts integer string to JSON number
- **WHEN** `inferTypes` is `true` and a CSV cell contains `42`
- **THEN** the response emits `42` as a JSON number (not `"42"`)

#### Scenario: Type inference converts decimal string to JSON number
- **WHEN** `inferTypes` is `true` and a CSV cell contains `3.14`
- **THEN** the response emits `3.14` as a JSON number using invariant-culture parsing

#### Scenario: Type inference converts "true"/"false" strings to JSON booleans
- **WHEN** `inferTypes` is `true` and a CSV cell contains `true` or `false` (case-insensitive)
- **THEN** the response emits the value as JSON boolean `true` or `false`

#### Scenario: Non-numeric strings remain strings with inferTypes true
- **WHEN** `inferTypes` is `true` and a CSV cell contains `hello`
- **THEN** the response emits `"hello"` as a JSON string

#### Scenario: Empty CSV cell emits empty string regardless of inferTypes
- **WHEN** a CSV cell is empty
- **THEN** the response emits `""` for that cell regardless of `inferTypes`

### Requirement: inferTypes is an optional request body field
The request record SHALL expose `inferTypes` as an optional boolean property with a default of `false`. It MUST NOT be a query parameter.

#### Scenario: Request without inferTypes field is accepted
- **WHEN** the request body does not include the `inferTypes` field
- **THEN** the endpoint processes the CSV and returns all values as JSON strings
