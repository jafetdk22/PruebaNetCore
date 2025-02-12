/*
Se decidió por SQL Server, ya que al ser tecnología de Microsoft,
tiene una gran compatibilidad con la suite de .NET, Azure, Power BI y otros productos empresariales.
Es adecuado para aplicaciones de misión crítica que requieren un manejo eficiente de grandes volúmenes de datos y transacciones.
*/
/* se decidio .Net core 9 Con C#
 C# es un lenguaje de programación potente, fácil de aprender y mantener. 
 Tiene características modernas como LINQ, async/await, y es ideal para aplicaciones de alto rendimiento.
 .Net 9 es la version mas resiente, la plataforma ofrece un rendimiento sobresaliente. 
 Gracias a mejoras en el runtime (CLR), la ejecución de código es más rápida.
 NET tiene un buen soporte para contenedores Docker, 
 lo que facilita la creación de aplicaciones que pueden escalarse de manera eficiente en un entorno de producción.
*/

-- Crear base de datos
CREATE DATABASE CompanyChargesDB;

-- Seleccionar base de datos
USE CompanyChargesDB;

/*Practica No.1*/

-- Eliminar tablas si existen
DROP TABLE IF EXISTS cargo;

-- Crear tabla Cargo
CREATE TABLE Cargo (
    id VARCHAR(255) NOT NULL PRIMARY KEY,
    company_name VARCHAR(255) NULL,
    company_id VARCHAR(255) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    status VARCHAR(30) NOT NULL, 
    created_at DATETIME2 NOT NULL,  -- Se usa DATETIME2 en lugar de TIMESTAMP
    updated_at DATETIME2 NULL
);

/*Para Crear el Stored Procedure descomentar las siguientes lineas*/

/*CREATE PROCEDURE GetCargoData AS
 BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        id, 
        company_name, 
        company_id, 
        amount, 
        status, 
        FORMAT(created_at, 'yyyy-MM-dd') AS created_at,
        FORMAT(updated_at, 'yyyy-MM-dd') AS updated_at
    FROM Cargo;
END;*/

/*Practica No.2*/

DROP TABLE IF EXISTS Charges;
DROP TABLE IF EXISTS Companies;
-- Crear tabla Companies
CREATE TABLE Companies (
    company_id VARCHAR(255) NOT NULL PRIMARY KEY,
    company_name VARCHAR(255) NOT NULL
);

-- Crear tabla Charges
CREATE TABLE Charges (
    charge_id VARCHAR(255) NOT NULL PRIMARY KEY,
    company_id VARCHAR(255) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    status VARCHAR(30) NOT NULL, 
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME2 NULL,
    FOREIGN KEY (company_id) REFERENCES Companies(company_id) ON DELETE CASCADE
);


/*Para Crear el Stored Procedure descomentar las siguientes lineas*/

/*CREATE PROCEDURE ImportChargesData
    @ChargeID VARCHAR(255),
    @CompanyID VARCHAR(255),
    @CompanyName VARCHAR(255),
    @Amount DECIMAL(18,2),
    @Status VARCHAR(30),
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2 NULL
 AS
 BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si la compañía existe
    IF NOT EXISTS (SELECT 1 FROM Companies WHERE company_id = @CompanyID)
    BEGIN
        -- Si la compañía no existe, crearla
        INSERT INTO Companies (company_id, company_name) 
        VALUES (@CompanyID, @CompanyName);
        PRINT 'Company created';
    END

    -- Verificar si charges ya existe
    IF NOT EXISTS (SELECT 1 FROM Charges WHERE charge_id = @ChargeID)
    BEGIN
        -- Insertar la transacción en Charges
        INSERT INTO Charges (charge_id, company_id, amount, status, created_at, updated_at)
        VALUES (@ChargeID, @CompanyID, @Amount, @Status, @CreatedAt, @UpdatedAt);
        PRINT 'Charge added';
    END
    ELSE
    BEGIN
        PRINT 'Charge ID already exists';
    END
END;*/


/*Para crear la vista descomentar las siguientes lineas*/

/*CREATE VIEW TotalChargesByDay AS
SELECT 
    c.company_id,
    c.company_name,
    CONVERT(VARCHAR(10), t.created_at, 23) AS transaction_date,
    SUM(t.amount) AS total_amount
 FROM 
    Charges t
 JOIN 
    Companies c ON t.company_id = c.company_id
 GROUP BY 
    c.company_id, c.company_name, CONVERT(VARCHAR(10), t.created_at, 23);
*/

