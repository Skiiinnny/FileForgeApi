## ADDED Requirements

### Requirement: config.yaml declara el stack tecnológico del proyecto
El `openspec/config.yaml` SHALL incluir en su campo `context` el stack completo: .NET 8, ASP.NET Core Minimal API, AWS Lambda (RestApi mode), MiniExcel, System.Text.Json con source generators, y PublishAot habilitado.

#### Scenario: Contexto incluye stack y restricciones AOT
- **WHEN** se lee el campo `context` del `config.yaml`
- **THEN** el texto incluye ".NET 8", "AWS Lambda", "MiniExcel", "PublishAot" y "AppJsonSerializerContext"

### Requirement: config.yaml documenta la arquitectura vertical slice
El `openspec/config.yaml` SHALL describir en el `context` el patrón de arquitectura: cada feature vive en `Features/<Name>/` con los archivos fijos Endpoint, Request, Response, Service, Service.Logging (partial), Validator, IService y DependencyInjection.

#### Scenario: Contexto describe los archivos de cada feature
- **WHEN** se lee el campo `context` del `config.yaml`
- **THEN** el texto enumera los 8 archivos del patrón vertical slice y su carpeta base `Features/`

### Requirement: config.yaml documenta las convenciones de dominio
El `openspec/config.yaml` SHALL incluir en el `context` las convenciones del dominio: Base64 para contenido de archivos, `Dictionary<string, string>` para filas, camelCase JSON, errores como `{ "error": "..." }`, y parámetros opcionales comunes (separator, encoding, newLine).

#### Scenario: Contexto incluye convenciones de request/response
- **WHEN** se lee el campo `context` del `config.yaml`
- **THEN** el texto menciona "Base64", "Dictionary<string, string>", "camelCase" y el formato de error

### Requirement: config.yaml define reglas por tipo de artefacto
El `openspec/config.yaml` SHALL incluir un campo `rules` con reglas específicas para artefactos `proposal` y `tasks` que refuercen los patrones del proyecto.

#### Scenario: Reglas para proposal incluyen restricción AOT
- **WHEN** se lee `rules.proposal` del `config.yaml`
- **THEN** contiene una regla que indica señalar si la feature requiere cambios en `AppJsonSerializerContext`

#### Scenario: Reglas para tasks incluyen los 8 archivos del patrón
- **WHEN** se lee `rules.tasks` del `config.yaml`
- **THEN** contiene una regla que indica crear los 8 archivos del patrón vertical slice y registrar en `Program.cs`
