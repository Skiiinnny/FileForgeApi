## Why

Las features de conversión repiten el mismo flujo base (validar entrada, resolver documento por URL cuando aplica, transformar y serializar salida) en múltiples `*Service`. Esta repetición dispersa invariantes críticos (orden de etapas, límites de tamaño, manejo homogéneo de errores) y permite que un mismo bug se replique en varios módulos.

## What Changes

- Extraer etapas comunes del pipeline de procesamiento a módulos reutilizables y de grano fino (resolución de documento, normalización tabular y paginación de filas).
- Recomponer los servicios de features (`Base64ToJson`, `Base64ToExcel`, `Base64ToCsv` y futuros equivalentes) para orquestar esas etapas compartidas en lugar de reimplementar el flujo.
- Estandarizar invariantes transversales: orden obligatorio de ejecución, validaciones de tamaño antes del parseo y política uniforme de errores.
- Consolidar pruebas de integración sobre las etapas compartidas y mantener pruebas ligeras por feature enfocadas en composición y contrato de endpoint.
- Mantener interfaces de feature pequeñas (un método) cuando aporte claridad, moviendo la complejidad al pipeline profundo reutilizable.

## Capabilities

### New Capabilities
- `shared-processing-pipeline-stages`: Etapas reutilizables para resolución de documento, normalización de tablas y paginación aplicable a filas.

### Modified Capabilities
- `base64-to-json`: La conversión usa etapas compartidas y hereda invariantes comunes de orden y validación.
- `base64-to-excel`: La conversión usa etapas compartidas para preparar datos y reducir duplicación de flujo.
- `base64-to-csv`: La conversión usa etapas compartidas para resolver entrada y normalizar datos antes de exportar.
- `url-document-fetch`: La resolución de documentos por URL se integra como etapa reutilizable del pipeline en lugar de lógica ad-hoc por feature.

## Impact

- Código afectado: `Features/*/*Service.cs`, servicios en `Shared/Documents/*` y validadores por feature vinculados al flujo.
- Pruebas afectadas: suites de `FileForgeApi.Tests/Features/*` y nuevas pruebas para etapas compartidas.
- API pública: no cambia el contrato HTTP esperado; se debe verificar y reflejar en `README.md` que no hay cambios funcionales visibles, sólo refactor estructural.
- Serialización AOT: no se esperan nuevos tipos en requests/responses; **no requiere cambios en** `Shared/Serialization/AppJsonSerializerContext.cs` salvo que durante la extracción se introduzca un DTO nuevo en contratos públicos.
