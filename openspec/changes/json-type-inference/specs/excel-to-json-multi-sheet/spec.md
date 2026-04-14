## ADDED Requirements

### Requirement: Response sheet rows always use JsonElement values
The endpoint SHALL return a map of type `Dictionary<string, List<Dictionary<string, JsonElement>>>`. When `inferTypes` is `false` (default), all values are JSON strings. When `inferTypes` is `true`, numeric and boolean cell values are emitted as their native JSON types, applied uniformly across all sheets.

#### Scenario: Default response emits all values as strings across all sheets
- **WHEN** `inferTypes` is omitted and multiple sheets contain numeric data
- **THEN** every value in every sheet is a JSON string

#### Scenario: Type inference applies to all sheets equally
- **WHEN** `inferTypes` is `true` and both sheet "A" and sheet "B" contain numeric cells
- **THEN** numeric values in both sheets are emitted as JSON numbers

#### Scenario: inferTypes false is equivalent to omitting the field
- **WHEN** `inferTypes` is explicitly `false`
- **THEN** the response is identical to omitting the field

#### Scenario: Sheet with no rows returns empty array for that sheet
- **WHEN** a sheet in the Excel file is empty (header row only or completely empty)
- **THEN** the corresponding key in the response maps to an empty array `[]`

### Requirement: inferTypes is an optional request body field
The request record SHALL expose `inferTypes` as an optional boolean property with a default of `false`. It MUST NOT be a query parameter.

#### Scenario: Request without inferTypes field is accepted
- **WHEN** the request body does not include the `inferTypes` field
- **THEN** the endpoint processes all sheets and returns all values as strings
