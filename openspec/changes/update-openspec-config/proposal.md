## Why

El archivo `openspec/config.yaml` del proyecto está vacío — solo tiene el schema declarado y el resto son comentarios de ejemplo. Sin contexto real del proyecto, los artefactos generados por OpenSpec (proposals, designs, tasks) no pueden tomar decisiones informadas sobre el stack, las convenciones ni la arquitectura de FileForgeApi.

## What Changes

- Completar el campo `context` del `config.yaml` con el stack tecnológico, la arquitectura vertical slice, las convenciones de código y el dominio del proyecto.
- Agregar `rules` por tipo de artefacto para reforzar patrones específicos del proyecto (AOT, registro en `AppJsonSerializerContext`, estructura de features, etc.).

## Capabilities

### New Capabilities

- `openspec-config`: Configuración de contexto y reglas de OpenSpec para FileForgeApi, que habilita la generación de artefactos informados y coherentes con el proyecto.

### Modified Capabilities

<!-- ninguna -->

## Impact

- Solo afecta al archivo `openspec/config.yaml`.
- No hay cambios en código de producción, tests ni infraestructura.
- Impacto directo en la calidad de todos los artefactos OpenSpec futuros generados para este proyecto.
