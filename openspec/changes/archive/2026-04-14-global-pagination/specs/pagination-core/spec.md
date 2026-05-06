## ADDED Requirements

### Requirement: Standard Pagination Parameters
The system SHALL provide a standard `PaginationParams` record to be used in request models. It MUST include `Page` (defaulting to 1) and `PageSize` (defaulting to 100).

#### Scenario: Default pagination values
- **WHEN** a request is made without providing `Page` or `PageSize`
- **THEN** the system SHALL use `Page = 1` and `PageSize = 100`

### Requirement: Standard Paginated Response
The system SHALL provide a standard `PaginatedResponse<T>` record to wrap collections in API responses. It MUST include:
- `Items`: The collection of type `T`.
- `TotalCount`: Total number of items in the source.
- `Page`: The current page number.
- `PageSize`: The number of items per page.
- `TotalPages`: Total number of pages available.
- `HasNextPage`: Boolean indicating if a next page exists.
- `HasPreviousPage`: Boolean indicating if a previous page exists.

#### Scenario: Successful paginated response construction
- **WHEN** a collection of 250 items is paginated with `PageSize = 100` and `Page = 1`
- **THEN** the response SHALL contain `Items` with 100 elements, `TotalCount = 250`, `Page = 1`, `PageSize = 100`, `TotalPages = 3`, `HasNextPage = true`, and `HasPreviousPage = false`
