## MODIFIED Requirements

### Requirement: Decodificar Base64 y retornar archivo JSON descargable
El sistema SHALL aceptar un objeto JSON con exactamente uno de los siguientes campos: `base64Content` (cadena Base64) o `documentUrl` (URL pública del documento). El sistema SHALL resolver la fuente de documento mediante una etapa compartida de pipeline antes de cualquier parseo/transformación, y SHALL retornar el contenido del documento como archivo JSON con cabeceras `Content-Type: application/json` y `Content-Disposition: attachment; filename=<filename>`. El parámetro `filename` sigue siendo opcional (default `"file.json"`).

#### Scenario: Descarga exitosa con base64Content y filename por defecto
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>" }`
- **THEN** la respuesta es HTTP 200 con `Content-Type: application/json`, `Content-Disposition: attachment; filename="file.json"` y el body es el contenido binario decodificado por la etapa compartida

#### Scenario: Descarga exitosa con base64Content y filename personalizado
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>", "filename": "datos.json" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="datos.json"` y el body es el contenido binario decodificado por la etapa compartida

#### Scenario: Descarga exitosa con documentUrl
- **WHEN** se envía `POST /api/base64/to-json` con `{ "documentUrl": "https://ejemplo.com/archivo.json" }`
- **THEN** el sistema resuelve el documento mediante la etapa compartida de URL y retorna HTTP 200 con `Content-Type: application/json` y el contenido binario del documento

#### Scenario: Descarga exitosa con documentUrl y filename personalizado
- **WHEN** se envía `POST /api/base64/to-json` con `{ "documentUrl": "https://ejemplo.com/archivo.json", "filename": "datos.json" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="datos.json"` y el contenido binario descargado por la etapa compartida
