## MODIFIED Requirements

### Requirement: Decodificar Base64 y retornar archivo Excel descargable
El sistema SHALL aceptar un objeto JSON con exactamente uno de los siguientes campos: `base64Content` (cadena Base64) o `documentUrl` (URL pública del documento). El sistema SHALL resolver la fuente de documento mediante una etapa compartida de pipeline antes de cualquier parseo/transformación, y SHALL retornar el contenido del documento como archivo Excel con cabeceras `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` y `Content-Disposition: attachment; filename=<filename>`. El parámetro `filename` sigue siendo opcional (default `"file.xlsx"`).

#### Scenario: Descarga exitosa con base64Content y filename por defecto
- **WHEN** se envía `POST /api/base64/to-excel` con `{ "base64Content": "<base64-xlsx>" }`
- **THEN** la respuesta es HTTP 200 con `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`, `Content-Disposition: attachment; filename="file.xlsx"` y el body es el contenido binario decodificado por la etapa compartida

#### Scenario: Descarga exitosa con base64Content y filename personalizado
- **WHEN** se envía `POST /api/base64/to-excel` con `{ "base64Content": "<base64-xlsx>", "filename": "informe.xlsx" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="informe.xlsx"` y el body es el contenido binario decodificado por la etapa compartida

#### Scenario: Descarga exitosa con documentUrl
- **WHEN** se envía `POST /api/base64/to-excel` con `{ "documentUrl": "https://ejemplo.com/archivo.xlsx" }`
- **THEN** el sistema resuelve el documento mediante la etapa compartida de URL y retorna HTTP 200 con `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` y el contenido binario del documento

#### Scenario: Descarga exitosa con documentUrl y filename personalizado
- **WHEN** se envía `POST /api/base64/to-excel` con `{ "documentUrl": "https://ejemplo.com/archivo.xlsx", "filename": "informe.xlsx" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="informe.xlsx"` y el contenido binario descargado por la etapa compartida
