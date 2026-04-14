## ADDED Requirements

### Requirement: Accept typed JSON values in row input
The endpoint SHALL accept rows of type `List<Dictionary<string, JsonElement>>`. Each `JsonElement` value is converted to its native CLR type before being passed to MiniExcel so that Excel cells reflect correct types (number cells store numbers, not text).

#### Scenario: String value is stored as text cell
- **WHEN** a row contains `{ "label": "hello" }` (JsonElement with ValueKind String)
- **THEN** the Excel cell for that column stores the text value `hello`

#### Scenario: Integer value is stored as numeric cell
- **WHEN** a row contains `{ "qty": 10 }` (JsonElement with ValueKind Number, integer)
- **THEN** the Excel cell stores the number `10` (not the text `"10"`)

#### Scenario: Decimal value is stored as numeric cell
- **WHEN** a row contains `{ "rate": 3.14 }` (JsonElement with ValueKind Number, decimal)
- **THEN** the Excel cell stores the floating-point value `3.14`

#### Scenario: Boolean value is stored as boolean cell
- **WHEN** a row contains `{ "enabled": true }` (JsonElement with ValueKind True)
- **THEN** the Excel cell stores the boolean value `TRUE`

#### Scenario: Null value results in empty cell
- **WHEN** a row contains `{ "comment": null }` (JsonElement with ValueKind Null)
- **THEN** the Excel cell is empty

#### Scenario: Mixed-type row round-trips through Excel
- **WHEN** a row contains `{ "name": "Bob", "score": 95, "pass": true, "note": null }`
- **THEN** the resulting Excel file contains a row with text, numeric, boolean, and empty cells respectively
