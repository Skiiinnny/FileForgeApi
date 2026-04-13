## MODIFIED Requirements

### Requirement: Decodificar Base64 y retornar archivo JSON descargable
El sistema SHALL aceptar un objeto JSON con exactamente uno de los siguientes campos: `base64Content` (cadena Base64) o `documentUrl` (URL pública del documento). El sistema SHALL retornar el contenido del documento como archivo JSON con cabeceras `Content-Type: application/json` y `Content-Disposition: attachment; filename=<filename>`. El parámetro `filename` sigue siendo opcional (default `"file.json"`).

#### Scenario: Descarga exitosa con base64Content y filename por defecto
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>" }`
- **THEN** la respuesta es HTTP 200 con `Content-Type: application/json`, `Content-Disposition: attachment; filename="file.json"` y el body es el contenido binario decodificado

#### Scenario: Descarga exitosa con base64Content y filename personalizado
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>", "filename": "datos.json" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="datos.json"` y el body es el contenido binario decodificado

#### Scenario: Descarga exitosa con documentUrl
- **WHEN** se envía `POST /api/base64/to-json` con `{ "documentUrl": "https://ejemplo.com/archivo.json" }`
- **THEN** el sistema descarga el documento desde la URL y retorna HTTP 200 con `Content-Type: application/json` y el contenido binario del documento

#### Scenario: Descarga exitosa con documentUrl y filename personalizado
- **WHEN** se envía `POST /api/base64/to-json` con `{ "documentUrl": "https://ejemplo.com/archivo.json", "filename": "datos.json" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="datos.json"` y el contenido binario descargado

## ADDED Requirements

### Requirement: Validación de exclusividad mutua de fuente de documento
El sistema SHALL rechazar requests que no cumplan la regla de "exactamente una fuente de documento".

#### Scenario: Base64Content vacío o ausente sin documentUrl
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "" }` o sin ninguno de los dos campos
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que se debe proporcionar exactamente uno de los dos campos

#### Scenario: Ambos campos presentes
- **WHEN** se envía `POST /api/base64/to-json` con `base64Content` y `documentUrl` ambos con valores
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que no se pueden proporcionar ambos campos

#### Scenario: Base64Content con cadena no válida
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "!!!not-base64!!!" }`
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que la cadena no es Base64 válido

#### Scenario: documentUrl con URL malformada
- **WHEN** se envía `POST /api/base64/to-json` con `{ "documentUrl": "not-a-url" }`
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que la URL no es válida

#### Scenario: documentUrl retorna error HTTP
- **WHEN** se envía `POST /api/base64/to-json` con una `documentUrl` que retorna HTTP 404
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que la URL retornó un código HTTP no exitoso
