## ADDED Requirements

### Requirement: Descargar documento desde URL pública
El sistema SHALL exponer un servicio `IDocumentFetchService` con un método `FetchAsync(string url)` que descargue el contenido binario de una URL pública y lo retorne como `byte[]`. El servicio SHALL validar que la URL sea HTTP o HTTPS y SHALL bloquear rangos IP privados/reservados (loopback, link-local, RFC-1918) para prevenir SSRF.

#### Scenario: Descarga exitosa
- **WHEN** se llama a `FetchAsync` con una URL HTTPS pública que retorna HTTP 200 y un body no vacío
- **THEN** el método retorna `Result<byte[]>.Success` con el contenido binario de la respuesta

#### Scenario: URL con esquema no permitido
- **WHEN** se llama a `FetchAsync` con una URL de esquema `ftp://` o `file://`
- **THEN** el método retorna `Result<byte[]>.Failure` con un mensaje indicando que solo se permiten URLs HTTP/HTTPS

#### Scenario: URL que apunta a IP privada (SSRF)
- **WHEN** se llama a `FetchAsync` con una URL que resuelve a `192.168.x.x`, `10.x.x.x`, `172.16-31.x.x`, `127.x.x.x` o `169.254.x.x`
- **THEN** el método retorna `Result<byte[]>.Failure` indicando que la URL apunta a una dirección no permitida

#### Scenario: Respuesta HTTP no exitosa
- **WHEN** la URL retorna un código HTTP distinto de 2xx (ej. 404, 403, 500)
- **THEN** el método retorna `Result<byte[]>.Failure` con un mensaje que incluye el código HTTP recibido

#### Scenario: Timeout de descarga
- **WHEN** la descarga no se completa dentro del tiempo límite configurado
- **THEN** el método retorna `Result<byte[]>.Failure` con un mensaje indicando que la descarga excedió el tiempo límite

#### Scenario: Respuesta vacía
- **WHEN** la URL retorna HTTP 200 pero el body está vacío
- **THEN** el método retorna `Result<byte[]>.Failure` indicando que el documento descargado está vacío

#### Scenario: Respuesta supera el tamaño máximo permitido
- **WHEN** el Content-Length de la respuesta supera el límite configurado (50 MB por defecto)
- **THEN** el método retorna `Result<byte[]>.Failure` indicando que el documento supera el tamaño máximo permitido

### Requirement: Validación de formato de URL en request
El sistema SHALL rechazar en la capa de validación del request toda URL que no sea sintácticamente válida (no parseable como `Uri` absoluta).

#### Scenario: URL malformada
- **WHEN** el campo `documentUrl` contiene un valor que no es una URI absoluta válida (ej. `"not-a-url"`, `"://missing-host"`)
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que la URL no es válida

### Requirement: Exactamente una fuente de documento requerida
Para todos los endpoints que aceptan `base64Content` o `documentUrl`, el sistema SHALL requerir que exactamente uno de los dos campos esté presente y no vacío. Proporcionar ambos o ninguno SHALL resultar en HTTP 400.

#### Scenario: Ningún campo presente
- **WHEN** se envía un request sin `base64Content` ni `documentUrl`
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que se debe proporcionar exactamente uno de los dos campos

#### Scenario: Ambos campos presentes simultáneamente
- **WHEN** se envía un request con `base64Content` y `documentUrl` ambos con valores no vacíos
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que no se pueden proporcionar ambos campos a la vez
