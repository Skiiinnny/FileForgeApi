## ADDED Requirements

### Requirement: Resolver la fuente de documento como etapa reutilizable
El sistema SHALL proveer una etapa compartida que reciba una entrada con `base64Content` o `documentUrl` y retorne un documento binario resuelto, aplicando la regla de exactamente una fuente y preservando errores normalizados de validación.

#### Scenario: Resolución desde base64Content
- **WHEN** una feature invoca la etapa con `base64Content` válido y sin `documentUrl`
- **THEN** la etapa retorna el contenido binario decodificado sin delegar a descarga por URL

#### Scenario: Resolución desde documentUrl
- **WHEN** una feature invoca la etapa con `documentUrl` válido y sin `base64Content`
- **THEN** la etapa delega en el servicio de descarga y retorna el contenido binario descargado

#### Scenario: Fuentes inválidas
- **WHEN** la entrada contiene ambos campos o ninguno
- **THEN** la etapa retorna falla de validación con mensaje de error consistente para todas las features consumidoras

### Requirement: Normalización y paginación tabular como etapas compartidas
El sistema SHALL proveer etapas compartidas para normalización de filas tabulares y paginación, de forma que cualquier feature que opere sobre tablas aplique el mismo orden de procesamiento e invariantes de límites.

#### Scenario: Normalización previa a serialización
- **WHEN** una feature transforma un documento a estructura tabular
- **THEN** la etapa de normalización produce filas en formato uniforme `Dictionary<string, string>` antes de serializar a JSON, CSV o Excel

#### Scenario: Paginación uniforme
- **WHEN** una feature solicita paginación de filas
- **THEN** la etapa aplica exactamente la misma semántica de `page/pageSize` y devuelve la misma selección de filas para iguales entradas
