
# Pasos para iniciar el proyecto

### 1.1 Crear la base de datos

1. Abre el archivo `CreateDataBase.sql` y ejecútalo para crear la base de datos.

### 1.2 Configurar la conexión a la base de datos

1. Abre el proyecto en Visual Studio.
2. Dirígete al archivo `appsettings.Development.json`.
3. Configura la cadena de conexión de la base de datos reemplazando lo siguiente:
   - `Data Source`: Si el servidor es local, usa `localhost`.
   - `Database`: El nombre de la base de datos debe coincidir con el creado en el paso 1.1.
   - `User Id`: Si el usuario no es el predeterminado `sa`, usa el usuario correspondiente para la conexión al servidor.
   - `Password`: Reemplázalo por la contraseña de acceso al servidor de base de datos.

   Ejemplo de configuración:
   ```json
   "ConnectionStrings": {
     "connectionDB": "Data Source=localhost;Database=CompanyChargesDB;User Id=sa;Password=TuContraseña;TrustServerCertificate=True;"
   }
   ```

### 1.3 Iniciar el proyecto

1. Inicia el proyecto en Visual Studio para que se abra Swagger.

---

# Pasos para usar el proyecto

## EJERCICIO 1

### 1.1 Importar los datos

1. Usa el endpoint `/api/v1/Data/Import` para subir el archivo `data_prueba_tecnica.csv`.

### 1.2 Exportar los datos limpios

1. Usa el endpoint `/api/v1/Data/export-csv` para exportar los datos limpios y preparados para el ejercicio 2. Esto devolverá un archivo llamado `cargo_data.csv`. Descárgalo.

## EJERCICIO 2

### 2.1 Importar los datos de cargos

1. Usa el endpoint `/api/v1/Charges/import` para subir el archivo descargado desde el endpoint `/api/v1/Data/export-csv`.

### 2.2 Calcular las transacciones diarias

1. Usa el endpoint `/api/v1/Calculation/daily-transactions` para calcular las transacciones diarias de cada empresa. Esto devolverá una lista con los resultados.

## EJERCICIO 3

### 3.1 Extraer un número de la lista

1. Usa el endpoint `/api/MissingNumber/extract` e inserta el número que deseas extraer de la lista, el cual debe estar entre 1 y 100.

### 3.2 Consultar el número eliminado

1. Usa el endpoint `/api/MissingNumber/missing` para consultar el número que fue eliminado de la lista.

---
