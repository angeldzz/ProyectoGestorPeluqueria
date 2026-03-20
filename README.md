# LookUp · Gestor de Peluquerías

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet" />
  <img src="https://img.shields.io/badge/ASP.NET-Core-6DB33F?style=for-the-badge" />
  <img src="https://img.shields.io/badge/SQL-Server-CC2927?style=for-the-badge&logo=microsoftsqlserver" />
  <img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap" />
  <img src="https://img.shields.io/badge/Leaflet-Map-199900?style=for-the-badge" />
</p>

Plataforma web para **gestionar peluquerías**, pensada para dos perfiles principales:

-  **Empresario**: registra su negocio, gestiona agenda, servicios, empleados y horarios.
-  **Cliente**: busca peluquerías, reserva citas y consulta su historial.
---
##  Galería visual

<p align="center"><b>Capturas reales del proyecto</b></p>

<table>
  <tr>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090240.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090430.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090553.png" width="100%"/></td>
  </tr>
  <tr>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090606.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090634.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090701.png" width="100%"/></td>
  </tr>
  <tr>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090728.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090743.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090809.png" width="100%"/></td>
  </tr>
  <tr>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090820.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090826.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090835.png" width="100%"/></td>
  </tr>
  <tr>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090846.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090911.png" width="100%"/></td>
    <td><img src="ProyectoGestorPeluqueria/wwwroot/imagenesDemo/Captura%20de%20pantalla%202026-03-20%20090921.png" width="100%"/></td>
  </tr>
</table>
---

##  Funcionalidades principales

### Empresario
- Alta de peluquería con logo, dirección y coordenadas.
- Gestión visual del calendario con **FullCalendar**.
- Alta y borrado de:
  - Servicios
  - Empleados
  - Tramos horarios (manual y por rango)
- Chat con clientes en detalle de peluquería.

### Cliente
- Exploración y búsqueda de peluquerías.
- Reserva de citas.
- Vista de **Mis Citas** (próximas + historial + calendario).
- Chat con la peluquería.

### Plataforma
- Autenticación por cookies.
- Políticas/autorización por rol.
- Persistencia en SQL Server.
- Mensajería con permisos por emisor.

---

##  Stack técnico

- `ASP.NET Core (.NET 10)`
- `Entity Framework Core`
- `SQL Server` (SP + vistas en BBDD)
- `Bootstrap 5` + `Font Awesome`
- `Leaflet` (mapas)
- `FullCalendar` (agenda)
- `SweetAlert2` (feedback visual)

---

##  Puesta en marcha

1. Clona el repositorio.
2. Configura la cadena de conexión `SqlServer` en `appsettings.json`.
3. Ejecuta la solución:

```bash
dotnet run --project ProyectoGestorPeluqueria
```

> El proyecto depende de tablas, vistas y procedimientos almacenados existentes en SQL Server y en los Repositories.

---

##  Notas

- El proyecto está en evolución continua.
- Incluye mejoras de UX/UI recientes en chat, agenda y layout global.
- Se recomienda ejecutar en entorno local con SQL Server compatible.

---

<p align="center"><b>Hecho con para digitalizar la gestión de peluquerías</b></p>
