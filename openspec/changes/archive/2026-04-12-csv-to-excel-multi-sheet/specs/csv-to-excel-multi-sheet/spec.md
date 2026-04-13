## ADDED Requirements

### Requirement: Convertir múltiples CSV a Excel multi-hoja
El sistema SHALL aceptar un objeto JSON donde cada clave es el nombre de una hoja y el valor es el contenido de un archivo CSV codificado en Base64, y SHALL devolver un único archivo Excel (.xlsx) codificado en Base64 con una hoja por cada entrada del mapa.

#### Scenario: Conversión exitosa con dos hojas
- **WHEN** se envía `POST /csv-to-excel-multi-sheet` con `{ "sheets": { "Ventas": "<base64-csv>", "Gastos": "<base64-csv>" } }`
- **THEN** la respuesta es HTTP 200 con `{ "fileBase64": "<base64-xlsx>" }` donde el Excel contiene las hojas "Ventas" y "Gastos" con los datos correspondientes

#### Scenario: Separador personalizado
- **WHEN** se envía la petición con query parameter `?separator=;`
- **THEN** el sistema usa `;` como separador al parsear todos los CSV del request

#### Scenario: Encoding personalizado
- **WHEN** se envía la petición con query parameter `?encoding=latin1`
- **THEN** el sistema decodifica el Base64 de cada hoja usando el encoding `latin1`

### Requirement: Validación del request
El sistema SHALL rechazar requests inválidos con HTTP 400 y un mensaje de error descriptivo.

#### Scenario: Mapa de hojas vacío
- **WHEN** se envía `{ "sheets": {} }` (diccionario vacío)
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que se requiere al menos una hoja

#### Scenario: Nombre de hoja vacío
- **WHEN** una de las claves del diccionario es una cadena vacía o solo espacios
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que el nombre de hoja no puede estar vacío

#### Scenario: Contenido CSV no es Base64 válido
- **WHEN** el valor de una hoja no es una cadena Base64 decodificable
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando la hoja problemática

#### Scenario: CSV con una sola fila (sin datos, solo encabezados)
- **WHEN** el CSV de una hoja contiene únicamente la fila de encabezados
- **THEN** el sistema genera la hoja con la fila de encabezados y cero filas de datos (sin error)

### Requirement: Preservar el orden de las hojas
El sistema SHALL respetar el orden de inserción de las entradas del diccionario al crear las hojas del Excel.

#### Scenario: Orden preservado
- **WHEN** el request contiene `{ "sheets": { "A": "...", "B": "...", "C": "..." } }`
- **THEN** el Excel resultante tiene las hojas en el orden A, B, C

### Requirement: Nombre de hoja con longitud máxima de Excel
El sistema SHALL rechazar nombres de hoja que superen los 31 caracteres (límite de Excel).

#### Scenario: Nombre de hoja demasiado largo
- **WHEN** una clave del diccionario tiene más de 31 caracteres
- **THEN** la respuesta es HTTP 400 con `{ "error": "..." }` indicando que el nombre excede el límite de Excel
