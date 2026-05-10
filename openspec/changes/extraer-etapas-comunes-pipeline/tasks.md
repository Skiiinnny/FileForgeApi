## 1. Infraestructura de etapas compartidas

- [ ] 1.1 Crear en `Shared` los contratos e implementaciones para etapas de pipeline: resolución de documento, normalización tabular y paginación de filas.
- [ ] 1.2 Definir el orden obligatorio de ejecución y las invariantes transversales (exactamente una fuente, límites de tamaño antes de parseo, errores homogéneos).
- [ ] 1.3 Registrar los nuevos servicios compartidos en DI y ajustar sus dependencias (`HttpClient`, opciones y logging).

## 2. Migración de features consumidoras

- [ ] 2.1 Refactorizar `Base64ToJsonService` para componer etapas compartidas sin cambiar contrato HTTP de `Base64ToJsonEndpoint`.
- [ ] 2.2 Refactorizar `Base64ToExcelService` para usar etapas compartidas y mantener el formato de archivo/cabeceras existente.
- [ ] 2.3 Refactorizar `Base64ToCsvService` para usar etapas compartidas y mantener el formato de archivo/cabeceras existente.
- [ ] 2.4 Alinear validadores por feature para que las reglas de entrada delegables al pipeline no se dupliquen ni contradigan.

## 3. Integración y contratos de aplicación

- [ ] 3.1 Verificar registro de feature en `Program.cs` (`AddX()` y `MapX()`) para todas las features impactadas por el refactor.
- [ ] 3.2 Revisar `Shared/Serialization/AppJsonSerializerContext.cs`; si aparecen nuevos tipos serializables en requests/responses, registrarlos explícitamente.
- [ ] 3.3 Actualizar `README.md` para documentar que el cambio es estructural (pipeline compartido) sin alterar la API pública.

## 4. Pruebas y validación de regresión

- [ ] 4.1 Crear pruebas de integración para las etapas compartidas (resolución de fuente, límites de tamaño, orden de ejecución, paginación).
- [ ] 4.2 Actualizar/agregar pruebas en `FileForgeApi.Tests/Features/Base64ToJson/`, `Base64ToExcel/` y `Base64ToCsv/` para verificar composición correcta y no-regresión funcional.
- [ ] 4.3 Ejecutar suite de tests afectada y corregir desvíos de comportamiento antes de cerrar el cambio.
