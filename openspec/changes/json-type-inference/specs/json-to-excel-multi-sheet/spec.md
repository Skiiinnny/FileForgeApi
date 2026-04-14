## ADDED Requirements

### Requirement: Accept typed JSON values per sheet
The endpoint SHALL accept a sheets map of type `Dictionary<string, List<Dictionary<string, JsonElement>>>`. Each sheet's rows follow the same typed-value semantics as the single-sheet `json-to-excel` feature: strings, numbers, booleans, and nulls are mapped to their native Excel cell types.

#### Scenario: String values stored as text across sheets
- **WHEN** multiple sheets each contain rows with string-valued `JsonElement` cells
- **THEN** all Excel cells in those sheets store text values

#### Scenario: Numeric values stored as number cells across sheets
- **WHEN** multiple sheets each contain rows with numeric `JsonElement` cells
- **THEN** all Excel cells store numeric values (not text)

#### Scenario: Null values produce empty cells across sheets
- **WHEN** a row in any sheet contains a null `JsonElement` value
- **THEN** the corresponding Excel cell is empty

#### Scenario: Each sheet preserves its own column types independently
- **WHEN** sheet "A" contains integer values in column "id" and sheet "B" contains string values in column "id"
- **THEN** "id" cells in sheet "A" are numeric and "id" cells in sheet "B" are text

#### Scenario: Validation fails when sheets map is empty
- **WHEN** the request contains `{ "sheets": {} }`
- **THEN** the endpoint returns 400 with `{ "error": "..." }`
