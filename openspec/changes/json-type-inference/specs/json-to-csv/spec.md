## ADDED Requirements

### Requirement: Accept typed JSON values in row input
The endpoint SHALL accept rows of type `List<Dictionary<string, JsonElement>>`, where each value may be a JSON string, number, boolean, or null. The existing validation rules (non-empty rows list, non-null request) remain unchanged.

#### Scenario: String value passes through unchanged
- **WHEN** a row contains `{ "col": "hello" }` (JsonElement with ValueKind String)
- **THEN** the CSV cell for that column contains `hello`

#### Scenario: Integer value is written as invariant-culture string
- **WHEN** a row contains `{ "amount": 42 }` (JsonElement with ValueKind Number, integer)
- **THEN** the CSV cell contains `42`

#### Scenario: Decimal value is written as invariant-culture string
- **WHEN** a row contains `{ "price": 9.99 }` (JsonElement with ValueKind Number, decimal)
- **THEN** the CSV cell contains `9.99` using invariant culture (period as decimal separator)

#### Scenario: Boolean true is written as lowercase string
- **WHEN** a row contains `{ "active": true }` (JsonElement with ValueKind True)
- **THEN** the CSV cell contains `true`

#### Scenario: Boolean false is written as lowercase string
- **WHEN** a row contains `{ "active": false }` (JsonElement with ValueKind False)
- **THEN** the CSV cell contains `false`

#### Scenario: Null value is written as empty string
- **WHEN** a row contains `{ "notes": null }` (JsonElement with ValueKind Null)
- **THEN** the CSV cell is empty

#### Scenario: Mixed-type row is fully written
- **WHEN** a row contains `{ "name": "Alice", "age": 30, "active": true, "score": null }`
- **THEN** the CSV row contains `Alice,30,true,` (using default separator)
