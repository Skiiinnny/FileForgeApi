# FileForgeApi

FileForgeApi is a lightweight, .NET 8 API for file conversions and spreadsheet operations, designed to run locally and on AWS Lambda with Native AOT support.

## What It Does

- Convert between Excel, CSV, JSON, and Base64 payloads
- Read Excel metadata
- Append rows to existing Excel files
- Generate Excel files from templates
- Parse tabular data to JSON with pagination support

## Tech Stack

- .NET 8 (`net8.0`)
- ASP.NET Core Minimal APIs
- Native AOT enabled
- AWS Lambda hosting support
- MiniExcel for spreadsheet handling
- Swagger UI available in `DEBUG`

## Run Locally

### Prerequisites

- .NET 8 SDK

### Start the API

```bash
dotnet restore
dotnet run
```

In `DEBUG`, Swagger UI is enabled at the app root.

## Run on AWS Lambda

This project is configured for Lambda hosting (`Amazon.Lambda.AspNetCoreServer.Hosting`) and Native AOT publish.

Typical flow:

1. Publish for Lambda runtime.
2. Package/deploy with your preferred AWS tooling (SAM, CDK, or AWS CLI pipeline).
3. Expose via API Gateway REST API.

## API Endpoints

### Excel

| Endpoint | Purpose | Typical Output |
|----------|---------|----------------|
| `POST /api/excel/to-json` | Convert Excel rows to JSON | `rows` (paginated) |
| `POST /api/excel/to-json/multi-sheet` | Convert Excel sheets to JSON | `sheets` (paginated per sheet) |
| `POST /api/excel/to-csv` | Convert Excel to CSV | `base64Content` |
| `POST /api/excel/metadata` | Extract Excel document metadata | metadata fields |
| `POST /api/excel/append-rows` | Append rows into an Excel file | `base64Content` |
| `POST /api/excel/from-template` | Fill template and generate Excel | `base64Content` |

### CSV

| Endpoint | Purpose | Typical Output |
|----------|---------|----------------|
| `POST /api/csv/to-json` | Convert CSV rows to JSON | `rows` (paginated) |
| `POST /api/csv/to-excel` | Convert CSV to Excel | `base64Content` |
| `POST /api/csv/to-excel/multi-sheet` | Convert CSV data into multi-sheet Excel | `fileBase64` |

### JSON

| Endpoint | Purpose | Typical Output |
|----------|---------|----------------|
| `POST /api/json/to-csv` | Convert JSON rows to CSV | `base64Content` |
| `POST /api/json/to-excel` | Convert JSON rows to Excel | `base64Content` |
| `POST /api/json/to-excel/multi-sheet` | Convert grouped JSON to multi-sheet Excel | `base64Content` |

### Base64

| Endpoint | Purpose |
|----------|---------|
| `POST /api/base64/to-csv` | Decode Base64 payload as CSV-oriented content |
| `POST /api/base64/to-excel` | Decode Base64 payload as Excel-oriented content |
| `POST /api/base64/to-json` | Decode Base64 payload as JSON-oriented content |

## Pagination

Pagination applies to:

- `POST /api/excel/to-json`
- `POST /api/csv/to-json`
- `POST /api/excel/to-json/multi-sheet` (per sheet)

### Request Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | `int?` | `1` | Page number (starts at 1) |
| `pageSize` | `int?` | `100` | Number of items per page |

Both values must be greater than `0` when provided.

### Response Shape

```json
{
  "rows": {
    "items": [],
    "totalCount": 1000,
    "page": 1,
    "pageSize": 100,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

For multi-sheet parsing, each value under `sheets` follows the same paginated structure.

## Request Rules and Limits

For endpoints that support file input via payload or URL:

- Provide exactly one source: `base64Content` or `documentUrl`
- Do not send both values simultaneously
- `documentUrl` must be a valid absolute URI
- `base64Content` must be a valid Base64 string

Operational limits currently configured:

- Remote document fetch timeout: 30 seconds
- Remote response buffer limit: 50 MB

## Minimal Request Examples

### Excel to JSON (paginated)

```json
{
  "base64Content": "<excel-file-base64>",
  "inferTypes": true,
  "page": 1,
  "pageSize": 100
}
```

### CSV to Excel

```json
{
  "base64Content": "<csv-file-base64>",
  "separator": ",",
  "encoding": "utf-8"
}
```

### JSON to CSV

```json
{
  "rows": [
    { "name": "Alice", "age": 30 },
    { "name": "Bob", "age": 28 }
  ],
  "separator": ",",
  "encoding": "utf-8"
}
```

## Testing

Run test suite with:

```bash
dotnet test
```
