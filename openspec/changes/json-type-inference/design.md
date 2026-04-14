## Context

Six JSON-related features use `Dictionary<string, string>` as their data row type — a domain convention established at project inception. This flat-string model was intentionally simple but now prevents consumers from sending or receiving typed values. The change upgrades the row value type from `string` to `JsonElement` across all six features without introducing new routes or dependencies.

AOT is the binding constraint: `System.Text.Json` source generators require every serialized type to be declared at compile time. `JsonElement` is a value type natively understood by the source generators, making it the only safe choice for a polymorphic cell value under AOT.

## Goals / Non-Goals

**Goals:**
- Allow input features (`json-to-csv`, `json-to-excel`, `json-to-excel-multi-sheet`) to accept numeric, boolean, and null JSON values, not only strings.
- Allow output features (`excel-to-json`, `excel-to-json-multi-sheet`, `csv-to-json`) to emit proper JSON types when `inferTypes: true` is supplied.
- Maintain AOT compatibility — no reflection-based serialization, no `object` in public API types.
- Backward-compatible deserialization for callers still sending string-only rows (a JSON string `"42"` remains a `JsonElement` with `ValueKind == String`).

**Non-Goals:**
- Schema negotiation or content negotiation (`Accept` headers, versioning).
- Deeply nested JSON — rows remain flat `Dictionary<string, JsonElement>`.
- Date/datetime inference (out of scope; too locale-sensitive).
- Changes to CSV encoding, separator, or encoding options.

## Decisions

### D1 — Use `JsonElement` as the cell value type (not `object?`, `JsonNode`, or a union record)

`JsonElement` is the only value type in `System.Text.Json` that can hold any JSON primitive (string, number, boolean, null) and is fully supported by AOT source generators without additional registration beyond the container types.

`object?` would require registering every concrete runtime type (`int`, `double`, `bool`, `string`, …) and is prone to silent AOT failures. `JsonNode` is a class hierarchy and requires a separate AOT registration pattern that is poorly supported in the current .NET 8 source-generator model.

**Row types after this change:**
| Feature direction | Type |
|---|---|
| Input rows | `List<Dictionary<string, JsonElement>>` |
| Multi-sheet input | `Dictionary<string, List<Dictionary<string, JsonElement>>>` |
| Output rows | `List<Dictionary<string, JsonElement>>` |
| Multi-sheet output | `Dictionary<string, List<Dictionary<string, JsonElement>>>` |

### D2 — Output features change response type unconditionally; `inferTypes` controls value content only

Rather than maintaining two response shapes depending on `inferTypes`, the response always uses `List<Dictionary<string, JsonElement>>`. When `inferTypes == false` (default), each `JsonElement` wraps a string (identical semantics to the old `Dictionary<string, string>`). When `inferTypes == true`, numeric and boolean values are represented as their native JSON types.

This keeps deserialization on the client side uniform and avoids a polymorphic response schema.

### D3 — Type inference logic is a shared static helper, not duplicated across three services

A `JsonTypeInferenceHelper.TryInfer(string raw, out JsonElement element)` static method (in `Shared/Json/`) attempts parsing in order: `null` → `bool` → `long` → `double` → fallback string. Each service that supports `inferTypes` calls this helper. All parsing uses `InvariantCulture` to avoid locale-dependent behavior.

### D4 — MiniExcel receives `object?` extracted from `JsonElement`, not `JsonElement` itself

MiniExcel's `SaveAsAsync` accepts `IEnumerable<IDictionary<string, object?>>`. Input services extract the native CLR value from each `JsonElement` before passing rows to MiniExcel:
- `JsonValueKind.String` → `string`
- `JsonValueKind.Number` → `double` (covers int and float; Excel stores all numbers as double)
- `JsonValueKind.True/False` → `bool`
- `JsonValueKind.Null` → `null`

This extraction is also a shared helper (`JsonElementExtensions.ToClrObject()`).

### D5 — `AppJsonSerializerContext` additions

Three new type registrations are required:
```csharp
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(List<Dictionary<string, JsonElement>>))]
[JsonSerializable(typeof(Dictionary<string, List<Dictionary<string, JsonElement>>>))]
```
The existing `Dictionary<string, string>` registrations are retained (used by other features and Lambda event types).

## Risks / Trade-offs

- **Double precision loss** → Mitigation: document that integer values exceeding `long.MaxValue` are not supported; for the Excel path they are stored as `double`, consistent with how Excel itself represents large numbers.
- **Silent AOT regression if new container types are added and not registered** → Mitigation: add an integration test that round-trips a typed payload end-to-end (serializes a request, calls the endpoint, deserializes the response) to catch missing registrations before deploy.
- **Behavior change for existing callers of output endpoints** → The response type changes from `Dictionary<string, string>` to `Dictionary<string, JsonElement>` even when `inferTypes` is omitted. Callers that deserialize using `Dictionary<string, string>` will receive a JSON type error for responses that contain non-string values. Mitigation: document the breaking change clearly; the default (`inferTypes: false`) means all values remain strings, so callers that do not touch `inferTypes` are unaffected if they deserialize loosely.
- **`JsonElement` lifetime** → `JsonElement` is backed by a `JsonDocument` that must not be disposed before the element is consumed. Because we parse request bodies through the ASP.NET Core model binder (which keeps the document alive for the request scope), this is safe. The inference helper must create its own `JsonDocument` per value and not dispose it prematurely; the returned `JsonElement` owns a copy of the data via `JsonElement.Clone()`.
