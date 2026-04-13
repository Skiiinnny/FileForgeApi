## 1. Modelos de datos

- [x] 1.1 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetRequest.cs` — record con `Dictionary<string, string> Sheets`
- [x] 1.2 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetResponse.cs` — record con `string FileBase64`

## 2. Interfaz e implementación del servicio

- [x] 2.1 Crear `Features/CsvToExcelMultiSheet/ICsvToExcelMultiSheetService.cs` — interfaz con método `ConvertAsync`
- [x] 2.2 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetService.cs` — implementación que decodifica cada CSV en Base64, lo parsea con el separador/encoding indicados y construye el Excel multi-hoja con MiniExcel
- [x] 2.3 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetService.Logging.cs` — partial class con el decorador de logging estructurado

## 3. Validación

- [x] 3.1 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetValidator.cs` con FluentValidation que valide:
  - `Sheets` no es nulo ni vacío (al menos una entrada)
  - Ninguna clave es cadena vacía o solo espacios
  - Ninguna clave supera 31 caracteres (límite de Excel)
  - Ningún valor es nulo o vacío

## 4. Endpoint y registro

- [x] 4.1 Crear `Features/CsvToExcelMultiSheet/CsvToExcelMultiSheetEndpoint.cs` — `MapCsvToExcelMultiSheet()` con ruta `POST /csv-to-excel-multi-sheet` y query params `separator` (default `,`) y `encoding` (default `utf-8`)
- [x] 4.2 Crear `Features/CsvToExcelMultiSheet/DependencyInjection.cs` — `AddCsvToExcelMultiSheet()` que registra el servicio y validador
- [x] 4.3 Registrar en `Program.cs`: llamar a `AddCsvToExcelMultiSheet()` y `MapCsvToExcelMultiSheet()`

## 5. Compatibilidad AOT

- [x] 5.1 Abrir `Shared/Serialization/AppJsonSerializerContext.cs` y verificar si `Dictionary<string, string>` ya está registrado
- [x] 5.2 Agregar atributos `[JsonSerializable(typeof(CsvToExcelMultiSheetRequest))]` y `[JsonSerializable(typeof(CsvToExcelMultiSheetResponse))]` (y cualquier tipo anidado necesario)

## 6. Tests de integración

- [x] 6.1 Crear `FileForgeApi.Tests/Features/CsvToExcelMultiSheet/` y el archivo de test principal
- [x] 6.2 Test: conversión exitosa con dos hojas distintas — verificar que el Excel resultante contiene ambas hojas con los datos correctos
- [x] 6.3 Test: separador personalizado (`;`) — verificar parsing correcto
- [x] 6.4 Test: request con `Sheets` vacío → HTTP 400
- [x] 6.5 Test: nombre de hoja con más de 31 caracteres → HTTP 400
- [x] 6.6 Test: nombre de hoja vacío → HTTP 400
- [x] 6.7 Test: CSV con solo encabezados (sin filas de datos) → HTTP 200, hoja con encabezados y cero filas
