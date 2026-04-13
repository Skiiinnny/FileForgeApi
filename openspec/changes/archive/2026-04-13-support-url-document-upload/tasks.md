## 1. Shared Infrastructure: UrlDocumentFetchService

- [x] 1.1 Crear `Shared/Documents/IDocumentFetchService.cs` con el método `Task<Result<byte[]>> FetchAsync(string url)`
- [x] 1.2 Crear `Shared/Documents/UrlDocumentFetchService.cs` que implemente `IDocumentFetchService` usando `HttpClient` (inyectado vía `IHttpClientFactory`)
- [x] 1.3 Implementar validación de esquema (solo `http`/`https`) y bloqueo de IPs privadas/reservadas en `UrlDocumentFetchService` antes de hacer la petición
- [x] 1.4 Implementar manejo de errores en `UrlDocumentFetchService`: respuesta no-2xx, timeout, respuesta vacía, tamaño máximo excedido (50 MB)
- [x] 1.5 Registrar `UrlDocumentFetchService` en `Program.cs` usando `builder.Services.AddHttpClient<UrlDocumentFetchService>()` con timeout configurado (30 s)
- [x] 1.6 Escribir tests unitarios para `UrlDocumentFetchService` en `FileForgeApi.Tests/Shared/Documents/` cubriendo todos los escenarios de la spec `url-document-fetch`

## 2. Feature: Base64ToCsv

- [x] 2.1 Actualizar `Base64ToCsvRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 2.2 Actualizar `Base64ToCsvValidator.cs`: reemplazar la validación de `Base64Content` obligatorio por la regla de exclusividad mutua (exactamente uno de los dos); añadir validación de URI absoluta para `DocumentUrl`; mantener validación de base64 bien formado cuando se usa `Base64Content`
- [x] 2.3 Actualizar `Base64ToCsvService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` desde `DocumentUrl` (via `IDocumentFetchService.FetchAsync`) o desde `Base64Content` (via `Convert.FromBase64String`) según el campo presente
- [x] 2.4 Actualizar `Base64ToCsvDependencyInjection.cs`: añadir `IDocumentFetchService` a la inyección de dependencias de la feature si aplica
- [x] 2.5 Actualizar tests en `FileForgeApi.Tests/Features/Base64ToCsv/`: añadir casos para flujo `documentUrl` (éxito, URL inválida, respuesta no-2xx, ambos campos, ningún campo)

## 3. Feature: Base64ToExcel

- [x] 3.1 Actualizar `Base64ToExcelRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 3.2 Actualizar `Base64ToExcelValidator.cs`: regla de exclusividad mutua, validación de URI absoluta para `DocumentUrl`, validación de base64 bien formado para `Base64Content`
- [x] 3.3 Actualizar `Base64ToExcelService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 3.4 Actualizar `Base64ToExcelDependencyInjection.cs` si es necesario
- [x] 3.5 Actualizar tests en `FileForgeApi.Tests/Features/Base64ToExcel/`: añadir casos para flujo `documentUrl`

## 4. Feature: Base64ToJson

- [x] 4.1 Actualizar `Base64ToJsonRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 4.2 Actualizar `Base64ToJsonValidator.cs`: regla de exclusividad mutua, validación de URI absoluta para `DocumentUrl`, validación de base64 bien formado para `Base64Content`
- [x] 4.3 Actualizar `Base64ToJsonService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 4.4 Actualizar `Base64ToJsonDependencyInjection.cs` si es necesario
- [x] 4.5 Actualizar tests en `FileForgeApi.Tests/Features/Base64ToJson/`: añadir casos para flujo `documentUrl`

## 5. Feature: CsvToExcel

- [x] 5.1 Actualizar `CsvToExcelRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 5.2 Actualizar `CsvToExcelValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 5.3 Actualizar `CsvToExcelService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 5.4 Actualizar `CsvToExcelDependencyInjection.cs` si es necesario
- [x] 5.5 Actualizar tests en `FileForgeApi.Tests/Features/CsvToExcel/`: añadir casos para flujo `documentUrl`

## 6. Feature: CsvToJson

- [x] 6.1 Actualizar `CsvToJsonRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 6.2 Actualizar `CsvToJsonValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 6.3 Actualizar `CsvToJsonService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 6.4 Actualizar `CsvToJsonDependencyInjection.cs` si es necesario
- [x] 6.5 Actualizar tests en `FileForgeApi.Tests/Features/CsvToJson/`: añadir casos para flujo `documentUrl`

## 7. Feature: ExcelToJson

- [x] 7.1 Actualizar `ExcelToJsonRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 7.2 Actualizar `ExcelToJsonValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 7.3 Actualizar `ExcelToJsonService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 7.4 Actualizar `ExcelToJsonDependencyInjection.cs` si es necesario
- [x] 7.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelToJson/`: añadir casos para flujo `documentUrl`

## 8. Feature: ExcelToJsonMultiSheet

- [x] 8.1 Actualizar `ExcelToJsonMultiSheetRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 8.2 Actualizar `ExcelToJsonMultiSheetValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 8.3 Actualizar `ExcelToJsonMultiSheetService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 8.4 Actualizar `ExcelToJsonMultiSheetDependencyInjection.cs` si es necesario
- [x] 8.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelToJsonMultiSheet/`: añadir casos para flujo `documentUrl`

## 9. Feature: ExcelToCsv

- [x] 9.1 Actualizar `ExcelToCsvRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 9.2 Actualizar `ExcelToCsvValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 9.3 Actualizar `ExcelToCsvService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 9.4 Actualizar `ExcelToCsvDependencyInjection.cs` si es necesario
- [x] 9.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelToCsv/`: añadir casos para flujo `documentUrl`

## 10. Feature: ExcelMetadata

- [x] 10.1 Actualizar `ExcelMetadataRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 10.2 Actualizar `ExcelMetadataValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 10.3 Actualizar `ExcelMetadataService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 10.4 Actualizar `ExcelMetadataDependencyInjection.cs` si es necesario
- [x] 10.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelMetadata/`: añadir casos para flujo `documentUrl`

## 11. Feature: ExcelAppendRows

- [x] 11.1 Actualizar `ExcelAppendRowsRequest.cs`: cambiar `Base64Content` a `string?` y añadir `string? DocumentUrl`
- [x] 11.2 Actualizar `ExcelAppendRowsValidator.cs`: regla de exclusividad mutua y validaciones por campo
- [x] 11.3 Actualizar `ExcelAppendRowsService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` según el campo presente
- [x] 11.4 Actualizar `ExcelAppendRowsDependencyInjection.cs` si es necesario
- [x] 11.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelAppendRows/`: añadir casos para flujo `documentUrl`

## 12. Feature: ExcelFromTemplate

- [x] 12.1 Actualizar `ExcelFromTemplateRequest.cs`: cambiar `Base64Template` a `string?` y añadir `string? TemplateUrl`
- [x] 12.2 Actualizar `ExcelFromTemplateValidator.cs`: regla de exclusividad mutua entre `Base64Template` y `TemplateUrl`; validaciones por campo
- [x] 12.3 Actualizar `ExcelFromTemplateService.cs`: inyectar `IDocumentFetchService`; resolver `byte[]` de la plantilla según el campo presente
- [x] 12.4 Actualizar `ExcelFromTemplateDependencyInjection.cs` si es necesario
- [x] 12.5 Actualizar tests en `FileForgeApi.Tests/Features/ExcelFromTemplate/`: añadir casos para flujo `templateUrl`

## 13. Documentación

- [x] 13.1 Actualizar `README.md`: documentar el nuevo campo `documentUrl` (o `templateUrl` para ExcelFromTemplate) en todos los endpoints afectados, indicando que es mutuamente excluyente con `base64Content`
