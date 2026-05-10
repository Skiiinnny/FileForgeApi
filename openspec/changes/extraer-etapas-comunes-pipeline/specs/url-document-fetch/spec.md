## MODIFIED Requirements

### Requirement: Descargar documento desde URL pública
El sistema SHALL exponer un servicio `IDocumentFetchService` con un método `FetchAsync(string url)` que descargue el contenido binario de una URL pública y lo retorne como `byte[]`. El servicio SHALL validar que la URL sea HTTP o HTTPS y SHALL bloquear rangos IP privados/reservados (loopback, link-local, RFC-1918) para prevenir SSRF. Además, SHALL poder utilizarse como etapa compartida del pipeline de procesamiento para todas las features que acepten `documentUrl`.

#### Scenario: Descarga exitosa
- **WHEN** se llama a `FetchAsync` con una URL HTTPS pública que retorna HTTP 200 y un body no vacío
- **THEN** el método retorna `Result<byte[]>.Success` con el contenido binario de la respuesta para ser consumido por la etapa compartida de resolución

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
