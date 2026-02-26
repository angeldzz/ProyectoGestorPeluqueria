DROP TABLE IF EXISTS MENSAJES;
DROP TABLE IF EXISTS CITAS;
DROP TABLE IF EXISTS HORARIOS_EMPLEADO;
DROP TABLE IF EXISTS EMPLEADOS;
DROP TABLE IF EXISTS SERVICIOS;
DROP TABLE IF EXISTS PELUQUERIAS;
DROP TABLE IF EXISTS USUARIOS_SECURITY;
DROP TABLE IF EXISTS USUARIOS;
DROP TABLE IF EXISTS ROLES;
DROP TABLE IF EXISTS ESTADOS_CITA;
GO
-- 1. Tabla de Roles
CREATE TABLE ROLES (
    RolID INT PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL
);
GO

-- 2. Tabla de Usuarios (Perfiles)
CREATE TABLE USUARIOS (
    UsuarioID INT PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    telefono NVARCHAR(20),
    RolID INT,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolID) REFERENCES ROLES(RolID)
);
GO
CREATE TABLE USUARIOS_SECURITY(
    UsuarioID INT PRIMARY KEY,
    Pass VARBINARY(MAX) NOT NULL,
    Salt NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_UsuariosSecurity_Usuarios FOREIGN KEY (UsuarioID) REFERENCES USUARIOS(UsuarioID)
);
-- 3. Tabla de Peluquerías
CREATE TABLE PELUQUERIAS (
    PeluqueriaID INT PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(255),
    UrlLogo NVARCHAR(MAX),
    Coordenadas NVARCHAR(100), -- Formato "Lat, Long"
    PropietarioID INT,
    CONSTRAINT FK_Peluquerias_Usuarios FOREIGN KEY (PropietarioID) REFERENCES USUARIOS(UsuarioID)
);
GO

-- 4. Tabla de Servicios
CREATE TABLE SERVICIOS (
    ServicioID INT PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    DuracionMin INT NOT NULL, -- Duraci?n en minutos
    PeluqueriaID INT,
    CONSTRAINT FK_Servicios_Peluquerias FOREIGN KEY (PeluqueriaID) REFERENCES PELUQUERIAS(PeluqueriaID) ON DELETE CASCADE
);
GO

-- 5. Tabla de Empleados
CREATE TABLE EMPLEADOS (
    EmpleadoID INT PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Activo BIT DEFAULT 1,
    PeluqueriaID INT,
    CONSTRAINT FK_Empleados_Peluquerias FOREIGN KEY (PeluqueriaID) REFERENCES PELUQUERIAS(PeluqueriaID) ON DELETE CASCADE
);
GO

-- 6. Tabla de Horarios de Empleado
CREATE TABLE HORARIOS_EMPLEADO (
    HorarioID         INT           IDENTITY(1,1) NOT NULL,
    EmpleadoID        INT           NOT NULL,
    FechaHoraApertura DATETIME      NOT NULL,
    FechaHoraCierre   DATETIME      NOT NULL,

    CONSTRAINT PK_Horarios
        PRIMARY KEY (HorarioID),

    CONSTRAINT FK_Horarios_Empleados
        FOREIGN KEY (EmpleadoID) REFERENCES dbo.EMPLEADOS(EmpleadoID)
        ON DELETE CASCADE,

    CONSTRAINT CHK_RangoHorario
        CHECK (FechaHoraCierre > FechaHoraApertura)
);
GO

-- 7. Tabla Maestra de Estados de Cita
CREATE TABLE ESTADOS_CITA (
    EstadoID INT PRIMARY KEY,
    NombreEstado NVARCHAR(50) NOT NULL
);
GO

-- 8. Tabla de Citas
CREATE TABLE CITAS (
    CitaID INT PRIMARY KEY,
    ClienteID INT,
    EmpleadoID INT,
    ServicioID INT,
    FechaHoraInicio DATETIME2 NOT NULL,
    FechaHoraFin DATETIME2 NOT NULL,
    NotasCliente NVARCHAR(MAX),
    EstadoID INT,
    CONSTRAINT FK_Citas_Clientes FOREIGN KEY (ClienteID) REFERENCES USUARIOS(UsuarioID),
    CONSTRAINT FK_Citas_Empleados FOREIGN KEY (EmpleadoID) REFERENCES EMPLEADOS(EmpleadoID),
    CONSTRAINT FK_Citas_Servicios FOREIGN KEY (ServicioID) REFERENCES SERVICIOS(ServicioID),
    CONSTRAINT FK_Citas_Estados FOREIGN KEY (EstadoID) REFERENCES ESTADOS_CITA(EstadoID),
    CONSTRAINT CK_Citas_FechaValida CHECK (FechaHoraFin > FechaHoraInicio)
);
GO

-- 9. Tabla de Mensajes
CREATE TABLE MENSAJES (
    MensajeID INT PRIMARY KEY,
    UsuarioID INT,
    PeluqueriaID INT,
    CitaID INT NULL, -- Referencia a cita si el mensaje está relacionado
    Mensaje NVARCHAR(MAX) NOT NULL,
    HoraCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Mensajes_Usuarios FOREIGN KEY (UsuarioID) REFERENCES USUARIOS(UsuarioID),
    CONSTRAINT FK_Mensajes_Peluquerias FOREIGN KEY (PeluqueriaID) REFERENCES PELUQUERIAS(PeluqueriaID),
    CONSTRAINT FK_Mensajes_Citas FOREIGN KEY (CitaID) REFERENCES CITAS(CitaID)
);
GO

---------------------------------------------------------------------
--CONSULTASINSERCION
---------------------------------------------------------------------
USE PROYECTOPELUQUERIA;
GO

-- Insertar Roles
INSERT INTO ROLES (RolID, NombreRol) VALUES 
(1, 'Administrador'), 
(2, 'Empresario'), 
(3, 'Cliente');

-- Insertar Usuarios (Password en texto plano solo para ejemplo, usar Hash en producci?n)
INSERT INTO USUARIOS (UsuarioID, Nombre, Password, email, telefono, RolID) VALUES 
(1, 'admin', '12345', 'admin@tajamar365.com', '643543445', 1),
(2, 'empresario', '12345', 'empresario@tajamar365.com', '616432345', 2),
(3, 'cliente', '12345', 'cliente@tajamar365.com', '653457654', 3);

-- Insertar Peluquería
INSERT INTO PELUQUERIAS (PeluqueriaID, Nombre, Direccion, UrlLogo, Coordenadas, PropietarioID) VALUES 
(1, 'Estilo & Corte', 'Calle Falsa 123, Madrid', 'http://logo.com/img.png', '40.4167,-3.7037', 2);

-- Insertar Servicios
INSERT INTO SERVICIOS (ServicioID, Nombre, Precio, DuracionMin, PeluqueriaID) VALUES 
(1, 'Corte de Pelo Caballero', 15.50, 30, 1),
(2, 'Tinte y Peinado', 45.00, 90, 1);

-- Insertar Empleados
INSERT INTO EMPLEADOS (EmpleadoID, Nombre, Activo, PeluqueriaID) VALUES 
(1, 'Carlos Peluquero', 1, 1),
(2, 'Ana Estilista', 1, 1);

-- Insertar Horarios (Lunes a Viernes para Carlos)
INSERT INTO HORARIOS_EMPLEADO (EmpleadoID, FechaHoraApertura, FechaHoraCierre) VALUES 
(1, CONVERT(DATETIME, '20260226 09:00:00', 120), CONVERT(DATETIME, '20260226 18:00:00', 120)),
(1, CONVERT(DATETIME, '20260227 09:00:00', 120), CONVERT(DATETIME, '20260227 18:00:00', 120));

-- Insertar Estados de Cita
INSERT INTO ESTADOS_CITA (EstadoID, NombreEstado) VALUES 
(1, 'Pendiente'), 
(2, 'Completada'), 
(3, 'Cancelada');

-- Insertar una Cita de ejemplo
INSERT INTO CITAS (CitaID, ClienteID, EmpleadoID, ServicioID, FechaHoraInicio, FechaHoraFin, NotasCliente, EstadoID) VALUES 
(1, 3, 1, 1, '2026-03-01 10:00:00', '2026-03-01 10:30:00', 'Quiero un corte degradado', 1);

-- Insertar un Mensaje
INSERT INTO MENSAJES (MensajeID, UsuarioID, PeluqueriaID, CitaID, Mensaje) VALUES 
(1, 3, 1, 1, 'Hola, ¿puedo cambiar mi cita para las 11?');
GO
------------------------------------------
--VIEWS
------------------------------------------

CREATE OR ALTER VIEW VW_MENSAJES_DETALLE AS
SELECT
    m.MensajeID,
    m.HoraCreacion,
    u.Nombre           AS NombreUsuario,
    u.email            AS EmailUsuario,
    p.Nombre           AS NombrePeluqueria,
    p.Direccion        AS DireccionPeluqueria,
    m.CitaID,
    c.FechaHoraInicio  AS CitaFechaInicio,
    c.FechaHoraFin     AS CitaFechaFin,
    ec.NombreEstado    AS EstadoCita,
    m.Mensaje
FROM dbo.MENSAJES          AS m
INNER JOIN dbo.USUARIOS    AS u   ON u.UsuarioID    = m.UsuarioID
INNER JOIN dbo.PELUQUERIAS AS p   ON p.PeluqueriaID = m.PeluqueriaID
LEFT  JOIN dbo.CITAS       AS c   ON c.CitaID        = m.CitaID
LEFT  JOIN dbo.ESTADOS_CITA AS ec ON ec.EstadoID     = c.EstadoID;
GO

CREATE OR ALTER VIEW VW_USUARIOS_CREDENCIALES AS
SELECT
    u.UsuarioID,
    u.Nombre,
    u.email,
    r.NombreRol,
    us.Pass,
    us.Salt
FROM dbo.USUARIOS            AS u
INNER JOIN dbo.ROLES          AS r  ON r.RolID      = u.RolID
INNER JOIN dbo.USUARIOS_SECURITY AS us ON us.UsuarioID = u.UsuarioID;
GO
CREATE OR ALTER VIEW VW_PELUQUERIA_DUENO_SERVICIOS AS
SELECT
    p.PeluqueriaID,
    p.Nombre           AS NombrePeluqueria,
    p.Direccion,
    p.Coordenadas,
    p.UrlLogo,
    u.UsuarioID        AS DuenoID,
    u.Nombre           AS NombreDueno,
    u.email            AS EmailDueno,
    u.telefono         AS TelefonoDueno,
    s.ServicioID,
    s.Nombre           AS NombreServicio,
    s.Precio,
    s.DuracionMin
FROM dbo.PELUQUERIAS        AS p
INNER JOIN dbo.USUARIOS     AS u ON u.UsuarioID    = p.PropietarioID
LEFT  JOIN dbo.SERVICIOS    AS s ON s.PeluqueriaID = p.PeluqueriaID;

CREATE OR ALTER VIEW VW_CITAS_CALENDARIO AS
SELECT
    c.CitaID            AS CitaId,
    c.ClienteID         AS ClienteId,
    c.EmpleadoID        AS EmpleadoId,
    c.ServicioID        AS ServicioId,
    c.EstadoID          AS EstadoId,
    e.PeluqueriaID      AS PeluqueriaId,
    c.FechaHoraInicio,
    c.FechaHoraFin,
    u.Nombre            AS NombreCliente,
    e.Nombre            AS NombreEmpleado,
    s.Nombre            AS NombreServicio,
    ec.NombreEstado,
    c.NotasCliente
FROM  dbo.CITAS c
LEFT JOIN dbo.USUARIOS     u  ON u.UsuarioID  = c.ClienteID
LEFT JOIN dbo.EMPLEADOS    e  ON e.EmpleadoID = c.EmpleadoID
LEFT JOIN dbo.SERVICIOS    s  ON s.ServicioID = c.ServicioID
LEFT JOIN dbo.ESTADOS_CITA ec ON ec.EstadoID  = c.EstadoID;
GO

------------------------------------------
--SELECTS VIEWS
------------------------------------------

SELECT * FROM VW_MENSAJES_DETALLE;
SELECT * FROM VW_USUARIOS_CREDENCIALES;
SELECT * FROM VW_PELUQUERIA_DUENO_SERVICIOS;
SELECT * FROM VW_CITAS_CALENDARIO;
------------------------------------------
--STORED PROCEDURES
------------------------------------------
GO
CREATE OR ALTER PROCEDURE SP_REGISTRAR_USUARIO
    @Nombre    NVARCHAR(100),
    @Password  NVARCHAR(100),
    @Email     NVARCHAR(100),
    @Telefono  NVARCHAR(20)   = NULL,
    @RolID     INT,
    @Pass      VARBINARY(MAX),
    @Salt      NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el email no esté ya registrado
    IF EXISTS (SELECT 1 FROM dbo.USUARIOS WHERE email = @Email)
    BEGIN
        RAISERROR('El email "%s" ya está registrado.', 16, 1, @Email);
        RETURN;
    END

    -- Verificar que el RolID existe
    IF NOT EXISTS (SELECT 1 FROM dbo.ROLES WHERE RolID = @RolID)
    BEGIN
        RAISERROR('El RolID %d no existe.', 16, 1, @RolID);
        RETURN;
    END

    DECLARE @NuevoUsuarioID INT;

    BEGIN TRANSACTION;
    BEGIN TRY

        -- Generar nuevo UsuarioID
        SELECT @NuevoUsuarioID = ISNULL(MAX(UsuarioID), 0) + 1
        FROM dbo.USUARIOS;

        -- Insertar en USUARIOS
        INSERT INTO dbo.USUARIOS (UsuarioID, Nombre, Password, email, telefono, RolID)
        VALUES (@NuevoUsuarioID, @Nombre, @Password, @Email, @Telefono, @RolID);

        -- Insertar credenciales de seguridad
        INSERT INTO dbo.USUARIOS_SECURITY (UsuarioID, Pass, Salt)
        VALUES (@NuevoUsuarioID, @Pass, @Salt);

        COMMIT TRANSACTION;

        -- Devolver el nuevo usuario creado
        SELECT
            u.UsuarioID,
            u.Nombre,
            u.email,
            u.telefono,
            r.NombreRol
        FROM dbo.USUARIOS AS u
        INNER JOIN dbo.ROLES AS r ON r.RolID = u.RolID
        WHERE u.UsuarioID = @NuevoUsuarioID;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

------------------------------------------
--SELECTS GENERALES
------------------------------------------
select * from MENSAJES
select * from CITAS
select * from HORARIOS_EMPLEADO
select * from EMPLEADOS
select * from SERVICIOS
select * from PELUQUERIAS
select * from USUARIOS_SECURITY
select * from USUARIOS
select * from ROLES
select * from ESTADOS_CITA
