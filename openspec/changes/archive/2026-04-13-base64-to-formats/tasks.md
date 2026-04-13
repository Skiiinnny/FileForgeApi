## 1. Feature Base64ToCsv

- [x] 1.1 Crear `Features/Base64ToCsv/Base64ToCsvRequest.cs` con propiedades `Base64Content` (string) y `Filename` (string?, default `"file.csv"`)
- [x] 1.2 Crear `Features/Base64ToCsv/Base64ToCsvResponse.cs` (record vacío — el response es binario, no JSON)
- [x] 1.3 Crear `Features/Base64ToCsv/IBase64ToCsvService.cs` con método `ConvertAsync(Base64ToCsvRequest?)`
- [x] 1.4 Crear `Features/Base64ToCsv/Base64ToCsvService.cs` — decodificar `Base64Content` con `Convert.FromBase64String` y retornar `Results.File(bytes, "text/csv", filename)`
- [x] 1.5 Crear `Features/Base64ToCsv/Base64ToCsvService.Logging.cs` — partial logging decorator con entrada/salida
- [x] 1.6 Crear `Features/Base64ToCsv/Base64ToCsvValidator.cs` — validar que `Base64Content` no sea vacío y sea Base64 válido
- [x] 1.7 Crear `Features/Base64ToCsv/Base64ToCsvEndpoint.cs` — ruta `POST /api/base64/to-csv`, tag `Base64`, handler con validación y llamada al servicio
- [x] 1.8 Crear `Features/Base64ToCsv/DependencyInjection.cs` — extensión `AddBase64ToCsv()`

## 2. Feature Base64ToExcel

- [x] 2.1 Crear `Features/Base64ToExcel/Base64ToExcelRequest.cs` con propiedades `Base64Content` (string) y `Filename` (string?, default `"file.xlsx"`)
- [x] 2.2 Crear `Features/Base64ToExcel/Base64ToExcelResponse.cs` (record vacío — response binario)
- [x] 2.3 Crear `Features/Base64ToExcel/IBase64ToExcelService.cs`
- [x] 2.4 Crear `Features/Base64ToExcel/Base64ToExcelService.cs` — decodificar y retornar `Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename)`
- [x] 2.5 Crear `Features/Base64ToExcel/Base64ToExcelService.Logging.cs`
- [x] 2.6 Crear `Features/Base64ToExcel/Base64ToExcelValidator.cs`
- [x] 2.7 Crear `Features/Base64ToExcel/Base64ToExcelEndpoint.cs` — ruta `POST /api/base64/to-excel`, tag `Base64`
- [x] 2.8 Crear `Features/Base64ToExcel/DependencyInjection.cs` — extensión `AddBase64ToExcel()`

## 3. Feature Base64ToJson

- [x] 3.1 Crear `Features/Base64ToJson/Base64ToJsonRequest.cs` con propiedades `Base64Content` (string) y `Filename` (string?, default `"file.json"`)
- [x] 3.2 Crear `Features/Base64ToJson/Base64ToJsonResponse.cs` (record vacío — response binario)
- [x] 3.3 Crear `Features/Base64ToJson/IBase64ToJsonService.cs`
- [x] 3.4 Crear `Features/Base64ToJson/Base64ToJsonService.cs` — decodificar y retornar `Results.File(bytes, "application/json", filename)`
- [x] 3.5 Crear `Features/Base64ToJson/Base64ToJsonService.Logging.cs`
- [x] 3.6 Crear `Features/Base64ToJson/Base64ToJsonValidator.cs`
- [x] 3.7 Crear `Features/Base64ToJson/Base64ToJsonEndpoint.cs` — ruta `POST /api/base64/to-json`, tag `Base64`
- [x] 3.8 Crear `Features/Base64ToJson/DependencyInjection.cs` — extensión `AddBase64ToJson()`

## 4. Registro y serialización

- [x] 4.1 Añadir `[JsonSerializable(typeof(Base64ToCsvRequest))]`, `[JsonSerializable(typeof(Base64ToExcelRequest))]` y `[JsonSerializable(typeof(Base64ToJsonRequest))]` en `Shared/Serialization/AppJsonSerializerContext.cs`
- [x] 4.2 Registrar los tres features en `Program.cs`: `builder.Services.AddBase64ToCsv()`, `AddBase64ToExcel()`, `AddBase64ToJson()`
- [x] 4.3 Mapear las rutas en `Program.cs`: `app.MapBase64ToCsv()`, `app.MapBase64ToExcel()`, `app.MapBase64ToJson()`

## 5. Tests de integración

- [x] 5.1 Crear `FileForgeApi.Tests/Features/Base64ToCsv/Base64ToCsvTests.cs` — casos: descarga exitosa sin filename, con filename, Base64 vacío, Base64 inválido
- [x] 5.2 Crear `FileForgeApi.Tests/Features/Base64ToExcel/Base64ToExcelTests.cs` — mismos casos para Excel
- [x] 5.3 Crear `FileForgeApi.Tests/Features/Base64ToJson/Base64ToJsonTests.cs` — mismos casos para JSON
- [x] 5.4 Verificar que los tests pasen con `dotnet test`

## 6. Documentación

- [x] 6.1 Actualizar `README.md` — añadir los tres nuevos endpoints en la sección de la API pública con descripción, request y response
