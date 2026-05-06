## Context

The API currently lacks pagination for collection-returning endpoints. Converting large Excel/CSV files to JSON results in large payloads that can strain both the server (AWS Lambda) and the client. This design introduces a standard, AOT-compatible pagination mechanism.

## Goals / Non-Goals

**Goals:**
- Define a standard `PaginatedResponse<T>` record.
- Update `ExcelToJson`, `CsvToJson`, and `ExcelToJsonMultiSheet` to support pagination.
- Ensure all new types are compatible with JSON source generation (AOT).
- Provide metadata: `TotalCount`, `Page`, `PageSize`, `TotalPages`, `HasNextPage`, `HasPreviousPage`.

**Non-Goals:**
- Pagination for endpoints returning single objects or files (e.g., `ExcelMetadata`, `JsonToExcel`).
- Complex filtering, searching, or sorting.
- Database-level pagination (the API is currently stateless and file-based).

## Decisions

### 1. Shared Infrastructure
Create `Shared/Pagination/PaginationParams.cs` and `Shared/Pagination/PaginatedResponse.cs`.
- `PaginationParams`: Defaults to `Page = 1`, `PageSize = 100`.
- `PaginatedResponse<T>`: Wraps the collection and adds metadata.

### 2. Request Convention
Pagination parameters will be added directly to the request records (e.g., `ExcelToJsonRequest`) as optional properties. This follows the existing project convention of avoiding `[FromQuery]` parameters in Minimal API handlers to keep handlers clean and easily testable.

### 3. In-Memory Pagination
Since files are processed in-memory via `MiniExcel`, we will:
1. Materialize the full `IEnumerable` into a `List` to get the `TotalCount`.
2. Apply `Skip((Page - 1) * PageSize).Take(PageSize)` to the list.
3. Construct the `PaginatedResponse`.

### 4. Multi-Sheet Pagination
For `ExcelToJsonMultiSheet`, the `Page` and `PageSize` in the request will apply to *every* sheet in the output. Each sheet's value in the response dictionary will be changed from `List<...>` to `PaginatedResponse<...>`.

### 5. AOT Registration
Every generic instantiation of `PaginatedResponse<T>` used in the API MUST be registered in `AppJsonSerializerContext.cs`.
- `PaginatedResponse<Dictionary<string, JsonElement>>`
- `PaginatedResponse<Dictionary<string, string>>` (if needed for internal or future use)

## Risks / Trade-offs

- **Memory Usage**: Materializing large lists in Lambda can be memory-intensive. 
  - *Mitigation*: The current 50MB file size limit helps bound memory usage. We will monitor memory usage and adjust Lambda limits if necessary.
- **Breaking Change**: Changing the response from `List<T>` to `PaginatedResponse<T>` is a breaking change for existing clients.
  - *Rationale*: This is necessary to introduce metadata. A major version bump or clear communication is required.
- **Uniform Multi-Sheet Pagination**: Users cannot request different pages for different sheets in a single call.
  - *Mitigation*: If this becomes a requirement, we can refine the request model later.
