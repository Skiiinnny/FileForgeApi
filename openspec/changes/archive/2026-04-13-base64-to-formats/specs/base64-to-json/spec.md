## ADDED Requirements

### Requirement: Decodificar Base64 y retornar archivo JSON descargable
El sistema SHALL aceptar un objeto JSON con `base64Content` (cadena Base64 obligatoria) y un parámetro opcional `filename`, y SHALL retornar el contenido decodificado como archivo JSON con cabeceras `Content-Type: application/json` y `Content-Disposition: attachment; filename=<filename>`.

#### Scenario: Descarga exitosa con filename por defecto
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>" }`
- **THEN** la respuesta es HTTP 200 con `Content-Type: application/json`, `Content-Disposition: attachment; filename="file.json"` y el body es el contenido binario decodificado

#### Scenario: Descarga exitosa con filename personalizado
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "<base64-json>", "filename": "datos.json" }`
- **THEN** la respuesta es HTTP 200 con `Content-Disposition: attachment; filename="datos.json"` y el body es el contenido binario decodificado

### Requirement: Validación del request
El sistema SHALL rechazar requests inválidos con HTTP 400 y un mensaje de error descriptivo.

#### Scenario: Base64Content vacío o ausente
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "" }` o sin el campo
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que el contenido Base64 es requerido

#### Scenario: Base64Content con cadena no válida
- **WHEN** se envía `POST /api/base64/to-json` con `{ "base64Content": "!!!not-base64!!!" }`
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que la cadena no es Base64 válido
