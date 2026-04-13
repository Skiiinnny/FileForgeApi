## Why

Actualmente todas las respuestas de la API devuelven el contenido como Base64. Los clientes que necesitan ofrecer descarga directa de archivos deben implementar la decodificación por su cuenta. Añadir endpoints que acepten una cadena Base64 y devuelvan el archivo binario en los formatos ya soportados (CSV, Excel, JSON) elimina esa fricción y completa el ciclo de la API.

## What Changes

- **Nuevo endpoint** `POST /api/base64/to-csv` — decodifica Base64 y devuelve el archivo como `text/csv` (descarga binaria).
- **Nuevo endpoint** `POST /api/base64/to-excel` — decodifica Base64 y devuelve el archivo como `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` (descarga binaria).
- **Nuevo endpoint** `POST /api/base64/to-json` — decodifica Base64 y devuelve el archivo como `application/json` (descarga binaria).
- Los tres endpoints admiten un parámetro opcional `filename` para el nombre del archivo en la cabecera `Content-Disposition`.
- Validación: Base64 inválido → HTTP 400 con `{ "error": "..." }`.
- `AppJsonSerializerContext` requiere registrar los nuevos tipos de Request. Los Response son `IResult` (archivo binario), por lo que no necesitan serialización JSON.

## Capabilities

### New Capabilities

- `base64-to-csv`: Decodifica una cadena Base64 y retorna el contenido como archivo CSV descargable.
- `base64-to-excel`: Decodifica una cadena Base64 y retorna el contenido como archivo Excel (.xlsx) descargable.
- `base64-to-json`: Decodifica una cadena Base64 y retorna el contenido como archivo JSON descargable.

### Modified Capabilities

## Impact

- **Nuevos features**: `Features/Base64ToCsv/`, `Features/Base64ToExcel/`, `Features/Base64ToJson/` — cada uno con los 8 archivos de la arquitectura vertical slice.
- **`Program.cs`**: registro de `AddX()` y `MapX()` para los tres nuevos features.
- **`Shared/Serialization/AppJsonSerializerContext.cs`**: añadir `Base64ToCsvRequest`, `Base64ToExcelRequest`, `Base64ToJsonRequest`.
- **`README.md`**: documentar los tres nuevos endpoints en la sección de la API pública.
- **Sin cambios en dependencias externas**: MiniExcel y System.Text.Json ya están disponibles; la decodificación usa `Convert.FromBase64String` del BCL.
