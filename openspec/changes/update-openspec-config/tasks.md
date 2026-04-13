## 1. Escribir el context del proyecto

- [x] 1.1 Agregar en `openspec/config.yaml` el bloque `context` con el stack tecnológico (.NET 8, ASP.NET Core Minimal API, AWS Lambda RestApi, MiniExcel, System.Text.Json AOT, PublishAot)
- [x] 1.2 Documentar en `context` la arquitectura vertical slice: carpeta `Features/<Name>/` con los 8 archivos del patrón (Endpoint, Request, Response, Service, Service.Logging, Validator, IService, DependencyInjection)
- [x] 1.3 Documentar en `context` las convenciones de dominio: Base64 para archivos, `Dictionary<string, string>` para filas, camelCase JSON, formato de error `{ "error": "..." }`, parámetros opcionales (separator, encoding, newLine)
- [x] 1.4 Documentar en `context` la restricción AOT: todo tipo nuevo en Request/Response debe registrarse en `AppJsonSerializerContext` (Shared/Serialization/)

## 2. Definir reglas por artefacto

- [x] 2.1 Agregar reglas para `proposal`: indicar si la feature requiere cambios en `AppJsonSerializerContext` e indicar si afecta el README
- [x] 2.2 Agregar reglas para `tasks`: desglosar siempre en los 8 archivos del patrón vertical slice + registro en Program.cs (AddX + MapX) + tarea de test en `FileForgeApi.Tests/Features/`

## 3. Verificación

- [x] 3.1 Revisar que el YAML generado sea válido (sin errores de sintaxis, indentación correcta)
- [x] 3.2 Confirmar que el contexto es legible y conciso (no más de ~40 líneas)
