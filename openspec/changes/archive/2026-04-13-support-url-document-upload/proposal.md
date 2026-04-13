## Why

Actualmente todos los endpoints que procesan documentos requieren que el archivo se envíe codificado en Base64, lo que fuerza a los consumidores a descargar y recodificar archivos remotos antes de llamar a la API. Permitir pasar una URL pública elimina esa fricción cuando el documento ya es accesible en internet.

## What Changes

- Todos los endpoints que aceptan `base64Content` (o `base64Template`) como entrada de archivo admitirán adicionalmente un campo opcional `documentUrl` con una URL pública de la que se descargará el documento.
- La validación requerirá que se proporcione exactamente uno de los dos campos (`base64Content` o `documentUrl`); enviar ambos o ninguno será un error 400.
- Se introducirá un servicio compartido `IDocumentFetchService` que encapsula la descarga HTTP, con manejo de errores uniforme (timeout, respuesta no-2xx, content-type inválido).
- Los tres endpoints que únicamente decodifican Base64 y retornan el archivo (`/api/base64/to-csv`, `/api/base64/to-excel`, `/api/base64/to-json`) también soportarán URL como fuente, obteniendo el contenido del documento desde la URL y devolviéndolo como descarga.
- Los endpoints de procesamiento sin spec formal (ExcelToJson, ExcelToCsv, ExcelMetadata, CsvToExcel, CsvToJson, ExcelAppendRows, ExcelFromTemplate, ExcelToJsonMultiSheet) recibirán el mismo cambio en sus modelos de request y validadores.

## Capabilities

### New Capabilities
- `url-document-fetch`: Servicio compartido que descarga el contenido de un documento desde una URL pública y lo expone como `byte[]`. Define los contratos de error (timeout, respuesta no-2xx, URL inválida) y los parámetros de configuración (timeout, tamaño máximo).

### Modified Capabilities
- `base64-to-csv`: El campo `base64Content` pasa a ser opcional; se añade `documentUrl` opcional. Exactamente uno de los dos debe estar presente.
- `base64-to-excel`: Mismo cambio que `base64-to-csv`.
- `base64-to-json`: Mismo cambio que `base64-to-csv`.

## Impact

- **Request models**: Todos los records con `Base64Content` (o `Base64Template`) obligatorio pasan a tener ambos campos como `string?` con validación de exclusividad mutua.
- **Validators**: Cada `FluentValidation` validator necesita la regla de "exactamente uno de los dos".
- **Services**: Los servicios existentes que llaman a `Convert.FromBase64String` deberán aceptar el `byte[]` ya resuelto (base64 decodificado o descargado), delegando la resolución al nuevo `IDocumentFetchService`.
- **AppJsonSerializerContext**: No se requieren nuevos tipos en `AppJsonSerializerContext`; los campos añadidos son `string?` que ya están cubiertos por los records existentes registrados.
- **README.md**: Afecta la documentación pública de todos los endpoints de procesamiento de documentos. Se debe actualizar para reflejar el nuevo campo `documentUrl`.
- **Dependencias**: Se requiere `HttpClient` / `IHttpClientFactory` configurado en DI para el `UrlDocumentFetchService`.
- **Tests**: Cada feature afectada necesita casos de test para el flujo URL (éxito, URL inválida, respuesta no-2xx, timeout).
