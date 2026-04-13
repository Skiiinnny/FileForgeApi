## Why

La API ya soporta `JsonToExcelMultiSheet` (múltiples arrays JSON → Excel multi-hoja) y `CsvToExcel` (CSV → Excel de una hoja), pero no existe un camino directo para convertir múltiples archivos CSV en un único Excel multi-hoja, dejando una brecha simétrica en la matriz de conversiones.

## What Changes

- Agregar el endpoint `POST /csv-to-excel-multi-sheet` que acepta un mapa de nombre-de-hoja → contenido CSV en Base64 y devuelve un único archivo Excel en Base64 con una hoja por cada entrada.
- Implementar la vertical slice completa de 8 archivos en `Features/CsvToExcelMultiSheet/`.
- Registrar el nuevo feature en `Program.cs` con `AddCsvToExcelMultiSheet()` y `MapCsvToExcelMultiSheet()`.
- Registrar los nuevos tipos serializables en `AppJsonSerializerContext`.

## Capabilities

### New Capabilities

- `csv-to-excel-multi-sheet`: Conversión de múltiples CSV (uno por hoja) a un único workbook Excel con soporte para separador y encoding configurables por hoja o globalmente.

### Modified Capabilities

<!-- ninguna -->

## Impact

- **AppJsonSerializerContext**: Requiere registro de `CsvToExcelMultiSheetRequest` y `CsvToExcelMultiSheetResponse` (y cualquier tipo anidado, como `Dictionary<string, CsvSheetInput>` si se introduce un wrapper).
- **API pública**: Añade un nuevo endpoint; el README.md debe actualizarse para documentarlo.
- **Sin breaking changes**: No modifica endpoints existentes.
- **Dependencias**: Ninguna nueva; usa MiniExcel y la infraestructura CSV ya presente en `CsvToExcel`.
