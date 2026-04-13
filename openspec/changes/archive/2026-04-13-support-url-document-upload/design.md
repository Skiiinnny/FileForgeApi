## Context

Actualmente todos los endpoints de FileForgeApi que procesan archivos reciben el contenido del documento codificado en Base64 dentro del body JSON. El flujo de resolución está acoplado directamente a `Convert.FromBase64String` en cada validator estático (`ValidateAndDecode`). No existe capa de abstracción para la fuente del documento.

El cambio introduce una segunda fuente: una URL pública desde la que se descarga el documento en tiempo de request. Esto afecta a 11 features de 15 (todas las que tienen `Base64Content` o `Base64Template` en su request model).

## Goals / Non-Goals

**Goals:**
- Permitir que cualquier endpoint que acepta `base64Content` también acepte `documentUrl` como fuente alternativa del documento.
- Encapsular la lógica de descarga HTTP en un único servicio compartido reutilizable por todas las features.
- Mantener la validación de "exactamente una fuente" consistente en todos los validators.
- No romper compatibilidad hacia atrás: requests que ya envían `base64Content` continúan funcionando sin cambios.

**Non-Goals:**
- Soporte de autenticación en URLs (sin headers, sin tokens OAuth).
- Cache de documentos descargados por URL.
- Soporte de URLs firmadas (S3 pre-signed, etc.) más allá de una HTTP GET simple.
- Modificar features cuyo input no es un archivo (JsonToCsv, JsonToExcel, JsonToExcelMultiSheet).

## Decisions

### D1: Servicio compartido en `Shared/` en lugar de duplicación por feature

**Decisión:** Crear `Shared/Documents/IDocumentFetchService.cs` y `UrlDocumentFetchService.cs` con la lógica de descarga HTTP.

**Alternativas consideradas:**
- Duplicar la lógica en cada feature — descartado: 11 features × mismo código HTTP = mantenimiento inviable.
- Extension method estático — descartado: dificulta el testing con mocks.

**Rationale:** El servicio compartido se registra en `Program.cs` una vez (`AddHttpClient<UrlDocumentFetchService>()`) y se inyecta vía interfaz en cada feature que lo necesite.

---

### D2: Los request models pasan de campo obligatorio a dos campos opcionales

**Decisión:** `Base64Content` pasa de `string` a `string?` y se añade `string? DocumentUrl`. Ambos son opcionales en el record; la exclusividad mutua se aplica en el validator.

```csharp
// Antes
public sealed record ExcelToJsonRequest(string Base64Content);

// Después
public sealed record ExcelToJsonRequest(
    string? Base64Content,
    string? DocumentUrl);
```

**Alternativas consideradas:**
- Un enum `DocumentSourceType` + campo `DocumentSource` — más explícito pero más verboso y rompe todos los tests.
- Un objeto discriminado `DocumentInput` — overkill para este tamaño de proyecto.

**Rationale:** Dos campos nullable con validación en FluentValidation es la representación más idiomática en Minimal API con source-gen AOT.

---

### D3: Resolución de bytes en el Service, no en el Validator

**Decisión:** Los validators pasan a validar solo la forma del request (presencia, formato URL, base64 bien formado). La resolución a `byte[]` —incluida la llamada async a `IDocumentFetchService`— ocurre en el `Service`.

**Alternativas consideradas:**
- Mantener `ValidateAndDecode` resolviendo bytes — descartado: la descarga HTTP es async, lo que haría async también al validator estático y complicaría el patrón.

**Rationale:** Separa responsabilidades: el validator garantiza que el request tiene sentido; el service obtiene y transforma los datos.

---

### D4: Usar `IHttpClientFactory` con cliente nombrado para `UrlDocumentFetchService`

**Decisión:** Registrar con `builder.Services.AddHttpClient<UrlDocumentFetchService>()` y configurar timeout y tamaño máximo de respuesta en el `HttpClient`.

**Rationale:** `IHttpClientFactory` gestiona el ciclo de vida del `HttpMessageHandler`, evitando socket exhaustion en Lambda. Configurar un timeout explícito (ej. 30 s) y un límite de tamaño de respuesta (ej. 50 MB) previene abusos.

---

### D5: Manejo de errores de descarga uniforme

**Decisión:** `UrlDocumentFetchService.FetchAsync` retorna `Result<byte[]>` con mensajes de error en español, igual que el resto de la API:
- URL inválida (no HTTP/HTTPS) → error de validación antes de hacer la petición.
- Respuesta no-2xx → `"La URL retornó el código HTTP {statusCode}."`.
- Timeout → `"La descarga del documento excedió el tiempo límite."`.
- Respuesta vacía → `"El documento descargado está vacío."`.

**Rationale:** Consistencia con el patrón `Result<T>` ya establecido en toda la codebase.

---

### D6: Sin cambios en AppJsonSerializerContext

**Decisión:** No es necesario registrar nuevos tipos. Los request records modificados siguen siendo `string?` que ya están cubiertos implícitamente. `IDocumentFetchService` no aparece en ningún tipo serializable.

## Risks / Trade-offs

- **[Risk] Lambda cold start más lento por HttpClient setup** → Mitigation: `AddHttpClient` con `IHttpClientFactory` ya está optimizado para Lambda; el impacto esperado es < 10ms.
- **[Risk] URLs con redirects o contenido demasiado grande** → Mitigation: configurar `MaxResponseContentBufferSize` en el `HttpClient` y no seguir más de N redirects.
- **[Risk] SSRF (Server-Side Request Forgery)** → Mitigation: validar que la URL sea `http://` o `https://` y bloquear rangos IP privados (10.x, 192.168.x, 172.16-31.x, 127.x, 169.254.x) en el servicio antes de hacer la petición.
- **[Risk] Features sin spec formal reciben cambio de contrato** → Mitigation: los tasks.md cubren todos los validators y services afectados; la ausencia de spec se compensa con los tests unitarios existentes y los nuevos casos de test URL.

## Migration Plan

1. Crear `Shared/Documents/IDocumentFetchService.cs` y `UrlDocumentFetchService.cs`.
2. Registrar `AddHttpClient<UrlDocumentFetchService>()` en `Program.cs` (junto a los `AddX()` existentes).
3. Para cada feature con `Base64Content`:
   a. Actualizar el request record (nullable + `DocumentUrl`).
   b. Refactorizar el validator: quitar lógica de decodificación, añadir reglas de exclusividad mutua y validación de URL.
   c. Refactorizar el service: resolver `byte[]` llamando a `IDocumentFetchService` o `Convert.FromBase64String` según el campo presente.
   d. Actualizar `DependencyInjection.cs` de la feature para inyectar `IDocumentFetchService`.
   e. Actualizar tests.
4. Actualizar `README.md`.

**Rollback:** El cambio es aditivo; si es necesario revertir, basta con eliminar los nuevos campos de los records y restaurar los validators. No hay cambios de base de datos ni estado externo.

## Open Questions

- ¿Se debe registrar en logs la URL descargada (puede contener tokens en query string)? → Propuesta: loguear solo el host, no la URL completa.
- ¿Cuál es el tamaño máximo aceptable de documento vía URL? → Propuesta inicial: 50 MB (igual que el límite de API Gateway para payloads).
