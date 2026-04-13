## Context

FileForgeApi expone una API de conversión de formatos de archivo. La arquitectura es vertical slice: cada feature vive en `Features/<Name>/` con exactamente 8 archivos y se registra en `Program.cs`. Ya existen `CsvToExcel` (CSV → Excel single-sheet) y `JsonToExcelMultiSheet` (JSON → Excel multi-hoja). Esta feature cierra la brecha simétrica: múltiples CSV → Excel multi-hoja.

El proyecto corre en AWS Lambda con PublishAot, lo que obliga a registrar todos los tipos serializables en `AppJsonSerializerContext` mediante source generators de System.Text.Json.

## Goals / Non-Goals

**Goals:**
- Implementar `POST /csv-to-excel-multi-sheet` siguiendo el patrón vertical slice existente.
- Reutilizar la lógica de parseo CSV ya validada en `CsvToExcelService`.
- Mantener compatibilidad AOT completa (sin reflexión dinámica).
- Soportar separador y encoding configurables de forma global para todas las hojas.

**Non-Goals:**
- Separador/encoding por hoja individual (se puede agregar en una iteración futura).
- Validación del contenido de los CSV más allá de que sean Base64 decodificables y parseables.
- Soporte para estilos o formatos de celda personalizados.

## Decisions

### Modelo de Request: `Dictionary<string, string>` como Sheets

**Decisión:** `CsvToExcelMultiSheetRequest` usará `Dictionary<string, string> Sheets` donde la clave es el nombre de la hoja y el valor es el contenido CSV en Base64.

**Alternativa considerada:** Un tipo wrapper `CsvSheetInput` con campos adicionales (separador por hoja). Descartado por ser prematuro — no hay requisito hoy y añade complejidad al registro AOT.

**Rationale:** Simetría directa con `JsonToExcelMultiSheetRequest` que usa `Dictionary<string, List<Dictionary<string, string>>>`. Los parámetros comunes (separator, encoding) van como query parameters, igual que en `CsvToExcel`.

### Reutilización del parseo CSV

**Decisión:** El servicio `CsvToExcelMultiSheetService` no dependerá de `ICsvToExcelService`; duplicará la lógica de parseo CSV inline o extraerá un helper estático compartido si el código es idéntico.

**Rationale:** La arquitectura vertical slice desacopla features deliberadamente. Inyectar un servicio de otro feature crea acoplamiento horizontal no previsto por el patrón. Si la duplicación resulta significativa, se puede extraer a `Shared/` en una refactor separada.

### Tipos en AppJsonSerializerContext

**Decisión:** Registrar explícitamente:
- `CsvToExcelMultiSheetRequest`
- `CsvToExcelMultiSheetResponse`
- `Dictionary<string, string>` (ya puede estar registrado; verificar en `AppJsonSerializerContext.cs`)

## Risks / Trade-offs

- **[Risk] Diccionario duplicado en AOT** → Si `Dictionary<string, string>` no está ya registrado en `AppJsonSerializerContext`, el build AOT fallará en runtime silenciosamente. Mitigación: revisar el archivo antes de implementar y agregar los atributos necesarios.
- **[Trade-off] Sin separador por hoja** → Simplifica el modelo pero limita casos de uso donde distintos CSV usan distintos separadores. Aceptable para v1; se puede extender con un `Dictionary<string, CsvSheetOptions>` en el futuro.
- **[Risk] Tamaño de payload** → Múltiples CSV en Base64 pueden superar el límite de 10 MB de API Gateway. Mitigación: documentar el límite; fuera de scope de esta feature agregar streaming.
