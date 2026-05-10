## Context

Los servicios de conversión en `Features/*/*Service.cs` comparten un patrón operativo casi idéntico, pero implementado localmente con variaciones pequeñas. Esto aumenta el costo de cambio y reduce la localización de invariantes críticos (por ejemplo: resolver documento antes de parsear, aplicar límites de tamaño de forma temprana, y mantener errores consistentes).  
El objetivo es introducir profundidad arquitectónica en `Shared/*` con etapas pequeñas y componibles, manteniendo la ergonomía de las features como orquestadores claros.

## Goals / Non-Goals

**Goals:**
- Definir un pipeline común reutilizable para resolución de documento, normalización tabular y paginación de filas.
- Reducir duplicación de flujo en `Base64ToJson`, `Base64ToExcel` y `Base64ToCsv`.
- Centralizar invariantes y su testeo de integración en un único lugar.
- Mantener interfaces por feature pequeñas, trasladando complejidad al módulo compartido.

**Non-Goals:**
- Cambiar contratos HTTP existentes o formato de respuestas.
- Sustituir completamente la estructura vertical-slice de features.
- Introducir nuevas dependencias externas de procesamiento de archivos.

## Decisions

1. **Pipeline por etapas explícitas en `Shared`**  
   Se crearán interfaces pequeñas (una responsabilidad por etapa), por ejemplo:
   - resolutor de documento de entrada (base64 directo o URL),
   - normalizador tabular (`Dictionary<string, string>` por fila),
   - aplicador de paginación por filas.
   **Rationale:** aumenta reutilización y testabilidad aislada.  
   **Alternativa considerada:** mantener helpers estáticos por feature; descartada por baja composabilidad y menor control de invariantes.

2. **Orquestación en servicios de feature, no en endpoints**  
   `*Endpoint` y validadores siguen delgados; `*Service` compone etapas compartidas.
   **Rationale:** respeta el patrón vertical-slice vigente y evita mover lógica de dominio hacia la capa HTTP.  
   **Alternativa considerada:** pipeline genérico único disparado desde endpoint; descartada por acoplar validación/transporte con transformación.

3. **Invariantes como contrato de etapa**  
   El orden de ejecución y validaciones transversales se definen en el módulo compartido (incluyendo límites antes de parseo pesado).
   **Rationale:** evita desviaciones por implementación local.  
   **Alternativa considerada:** documentar convenciones sin enforcement; descartada por fragilidad y regresiones repetidas.

4. **Estrategia de pruebas en dos niveles**  
   - Integración de etapas compartidas para validar invariantes una vez.
   - Pruebas de feature enfocadas en composición y outputs esperados.
   **Rationale:** cobertura más robusta con menor redundancia.

## Risks / Trade-offs

- **[Riesgo] Sobre-abstracción temprana** → **Mitigación:** iniciar con 3 etapas concretas usadas por features existentes y evitar capas genéricas sin consumidores reales.
- **[Riesgo] Acoplamiento accidental entre etapas** → **Mitigación:** contratos mínimos y DTOs internos estables, con tests por etapa.
- **[Riesgo] Regresiones por migración gradual** → **Mitigación:** migrar feature por feature con tests de no-regresión antes y después de cada extracción.
- **[Trade-off] Más tipos en `Shared`** → **Mitigación:** convenciones de naming y carpetas por etapa para mantener navegabilidad.

## Migration Plan

1. Implementar etapas compartidas en `Shared` y sus pruebas de integración.
2. Migrar `Base64ToJsonService` a composición de etapas y validar tests.
3. Migrar `Base64ToExcelService` y validar tests.
4. Migrar `Base64ToCsvService` y validar tests.
5. Limpiar duplicaciones residuales y actualizar README si cambia documentación operativa.

Rollback: si una migración rompe paridad funcional, restaurar temporalmente la implementación previa del servicio afectado y mantener etapas compartidas sin adopción hasta corregir.

## Open Questions

- ¿Los límites de tamaño se fijarán por tipo de documento o como política única global?
- ¿La etapa de paginación debe aplicarse antes o después de una normalización que elimine filas vacías?
- ¿Conviene exponer métricas por etapa (duración/errores) desde esta iteración o dejarlo para un cambio posterior?
