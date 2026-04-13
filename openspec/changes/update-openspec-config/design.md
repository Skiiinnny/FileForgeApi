## Context

`openspec/config.yaml` es el archivo de configuración raíz de OpenSpec para este proyecto. Actualmente solo declara `schema: spec-driven` y el resto son comentarios de ejemplo sin contenido real. Esto hace que cualquier artefacto generado opere sin conocimiento del proyecto.

El proyecto FileForgeApi tiene características específicas importantes que un LLM necesita conocer para tomar buenas decisiones:
- AOT compilation (limita qué librerías/patterns son válidos)
- Source generators para JSON serialization (todo tipo nuevo requiere registro)
- Arquitectura vertical slice muy uniforme (patrón rígido a respetar)
- Deploy en AWS Lambda (afecta decisiones de startup, cold start, etc.)

## Goals / Non-Goals

**Goals:**
- Documentar en `config.yaml` el stack completo, arquitectura y convenciones del proyecto
- Definir reglas por tipo de artefacto que refuercen los patrones del proyecto
- Hacer que los futuros artefactos OpenSpec sean coherentes con las decisiones ya tomadas

**Non-Goals:**
- No modificar código de producción
- No agregar nuevas features ni cambiar comportamiento de la API
- No crear specs para features existentes (eso sería un change separado)

## Decisions

### Contenido del `context`

El campo `context` incluirá:

1. **Stack tecnológico**: .NET 8, ASP.NET Core Minimal API, AWS Lambda (RestApi), MiniExcel, System.Text.Json con source generators, PublishAot
2. **Arquitectura**: Vertical slice — cada feature en `Features/<Name>/` con archivos fijos (Endpoint, Request, Response, Service, Service.Logging, Validator, IService, DependencyInjection)
3. **Convenciones de dominio**: Base64 para archivos, `Dictionary<string, string>` para filas, camelCase JSON, errores como `{ "error": "..." }`
4. **Restricciones AOT**: Todo tipo serializado debe estar en `AppJsonSerializerContext`
5. **Convención de parámetros opcionales**: separator, encoding, newLine con sus defaults

### Reglas por artefacto

- **proposal**: señalar si requiere cambios en `AppJsonSerializerContext` y si afecta el README
- **tasks**: desglosar siempre en los 8 archivos del patrón vertical slice + registro en Program.cs + tests

## Risks / Trade-offs

- **Contexto se puede desactualizar**: Si el proyecto evoluciona (nuevo patrón, nueva lib), el `config.yaml` debe actualizarse manualmente. → Mitigación: tratar el config como documentación viva, actualizar en cada change relevante.
- **Contexto demasiado largo**: Un context muy extenso puede perjudicar la señal/ruido. → Mitigación: mantenerlo conciso, priorizar lo que no es obvio leyendo el código.
