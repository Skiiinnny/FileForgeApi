## Why

All JSON-related endpoints currently treat every cell value as `string`, using `Dictionary<string, string>` for rows. This forces consumers to do their own type coercion and produces incorrect Excel output (numbers and booleans stored as text). Enabling type inference closes the gap between what the JSON data model can express and what these endpoints actually handle.

## What Changes

- **Input features** (`json-to-csv`, `json-to-excel`, `json-to-excel-multi-sheet`): Replace `Dictionary<string, string>` rows with `Dictionary<string, JsonElement>` so callers can send typed JSON values (numbers, booleans, nulls). Existing string-only clients are unaffected because a JSON string still deserializes to a `JsonElement` with `ValueKind == String`. **BREAKING** for any client that relied on the exact request schema name or generated client SDK types.
- **Output features** (`excel-to-json`, `excel-to-json-multi-sheet`, `csv-to-json`): Add an optional `inferTypes` boolean field (default `false`). When `true`, numeric and boolean cell values are emitted as proper JSON types instead of strings. The response row type changes from `Dictionary<string, string>` to `Dictionary<string, JsonElement>`. **BREAKING** when `inferTypes` is omitted/`false` — response schema changes even without type inference being active, so consumers must update deserialization.
- `AppJsonSerializerContext` gains registrations for `Dictionary<string, JsonElement>`, `List<Dictionary<string, JsonElement>>`, and `Dictionary<string, List<Dictionary<string, JsonElement>>>`.
- No new endpoints are introduced; no routes change.

## Capabilities

### New Capabilities

_None — this change modifies existing feature behavior only._

### Modified Capabilities

- `json-to-csv`: Request rows change from `List<Dictionary<string, string>>` to `List<Dictionary<string, JsonElement>>`. Values are serialized to string for CSV output; numbers/booleans use invariant culture formatting.
- `json-to-excel`: Same row type change as `json-to-csv`; MiniExcel receives `object?` values extracted from `JsonElement` so numeric cells are stored as numbers in the workbook.
- `json-to-excel-multi-sheet`: Sheets value type changes from `List<Dictionary<string, string>>` to `List<Dictionary<string, JsonElement>>`.
- `excel-to-json`: Adds optional `inferTypes: bool` (default `false`). Response rows change to `List<Dictionary<string, JsonElement>>` in all cases.
- `excel-to-json-multi-sheet`: Same `inferTypes` flag and response type change as `excel-to-json`.
- `csv-to-json`: Adds optional `inferTypes: bool` (default `false`). Response rows change to `List<Dictionary<string, JsonElement>>` in all cases.

## Impact

- **Code**: Six feature services and their request/response records, plus `AppJsonSerializerContext`.
- **API**: All six endpoints retain their existing routes and HTTP verbs. Request bodies and response bodies gain type breadth.
- **AppJsonSerializerContext** (`Shared/Serialization/AppJsonSerializerContext.cs`): Must register `Dictionary<string, JsonElement>`, `List<Dictionary<string, JsonElement>>`, and `Dictionary<string, List<Dictionary<string, JsonElement>>>`. Existing `Dictionary<string, string>` registrations remain.
- **README.md**: Request/response type documentation for all six endpoints must be updated to reflect the new schemas.
- **Tests**: Existing tests must be updated; new typed-value test cases added for each feature.
- **Dependencies**: No new NuGet packages — `System.Text.Json` already provides `JsonElement`.
