## Context

La API ya transmite todo el contenido de archivos como Base64. Los clientes obtienen un Base64 de respuesta y deben decodificarlo ellos mismos para generar una descarga. El cambio introduce tres nuevos features verticales (`Base64ToCsv`, `Base64ToExcel`, `Base64ToJson`) que aceptan una cadena Base64 y devuelven el contenido binario directamente como HTTP response con las cabeceras `Content-Type` y `Content-Disposition` adecuadas.

## Goals / Non-Goals

**Goals:**
- Endpoints que decodifican Base64 y retornan archivo binario descargable en CSV, Excel y JSON.
- Validación de Base64 malformado con HTTP 400.
- Parámetro opcional `filename` para personalizar `Content-Disposition`.
- Mantener consistencia total con la arquitectura vertical slice de 8 archivos.
- Registrar los Request types en `AppJsonSerializerContext` (requisito AOT).

**Non-Goals:**
- Detección automática del tipo de archivo contenido en el Base64.
- Validación del contenido del archivo (p. ej., que sea un Excel bien formado).
- Conversión entre formatos (eso ya lo hacen los endpoints existentes).
- Soporte para Base64 con data URIs (`data:application/...;base64,...`).

## Decisions

### D1: Response como `Results.File(bytes, contentType, filename)` en lugar de JSON

**Decisión:** Usar `Results.File` de ASP.NET Core para retornar los bytes directamente.

**Rationale:** Los endpoints existentes retornan `{ "base64Content": "..." }`. Para la descarga binaria, envolver los bytes en JSON doblaría el tamaño y rompería la semántica. `Results.File` produce la respuesta binaria con `Content-Disposition: attachment` de forma nativa.

**Alternativa descartada:** Retornar un JSON con `{ "base64Content": "..." }` sería idéntico al origen y no añadiría valor.

### D2: Tres features independientes en lugar de un endpoint genérico con parámetro `format`

**Decisión:** Crear `Base64ToCsv`, `Base64ToExcel` y `Base64ToJson` como features separados.

**Rationale:** Sigue el patrón establecido del proyecto (un endpoint = un feature = 8 archivos). Un endpoint genérico requeriría discriminadores en runtime, lógica de ramificación y registros extra en `AppJsonSerializerContext`, complicando la slice sin beneficio real. Las tres rutas son explícitas y autodocumentadas.

**Alternativa descartada:** `POST /api/base64/to-file?format=csv|excel|json` — más conciso pero rompe la convención del proyecto y complica tests y validación.

### D3: `Base64ToXxxResponse` como tipo vacío (wrapper de conveniencia)

**Decisión:** El archivo de `Response.cs` define un record vacío o simplemente documenta que el response es binario. El handler retorna `IResult` directamente.

**Rationale:** Los endpoints de descarga no producen un body JSON, pero la arquitectura exige el archivo `Response.cs`. Se mantiene el contrato estructural con un record marcado como `// Binary response — no JSON body` para claridad.

### D4: Validación solo de Base64 bien formado, no del contenido

**Decisión:** El `Validator` verifica únicamente que `Base64Content` no sea vacío y que `Convert.FromBase64String` no lance excepción.

**Rationale:** Validar que el contenido sea un Excel o CSV válido requeriría parsear el archivo en la capa de validación, duplicando lógica del servicio. Si el archivo está corrupto, el cliente recibirá un archivo corrupto — comportamiento aceptable y alineado con el resto de la API (GIGO).

## Risks / Trade-offs

- **[Riesgo] AOT + `AppJsonSerializerContext`:** Si se olvida registrar algún tipo de Request, la serialización falla silenciosamente en runtime Lambda. → Mitigación: la tarea de implementación incluye explícitamente este paso para los tres tipos.

- **[Trade-off] Archivos grandes en memoria:** `Convert.FromBase64String` materializa el array de bytes completo en memoria. Para archivos grandes esto puede ser un problema en Lambda con memoria limitada. → Aceptado: es el patrón actual de toda la API y no se introduce nada nuevo.

- **[Riesgo] `Content-Disposition` filename con caracteres especiales:** El parámetro `filename` podría contener caracteres no ASCII. → Mitigación: usar `ContentDispositionHeaderValue` con `FileNameStar` (RFC 5987) o simplemente sanear el nombre en el servicio.
