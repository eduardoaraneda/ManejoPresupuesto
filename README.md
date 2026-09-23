# Manejo de Presupuesto

Aplicación ASP.NET Core MVC para organizar cuentas, categorías, ingresos y gastos, consultar transacciones y generar reportes.

## Tecnologías

C# · .NET 10 · ASP.NET Core MVC · Razor · SQL Server · Dapper · AutoMapper · ClosedXML. Autenticación con Identity, un almacén de usuarios personalizado y cookies.

## Organización

- `Controllers/`: usuarios, transacciones, cuentas, tipos de cuenta y categorías.
- `Servicios/`: acceso a datos, reportes, identidad y correo.
- `Models/`, `Validaciones/` y `Views/`: modelos, reglas de validación y presentación.

## Preparar el entorno

Necesitas el SDK de .NET 10 y SQL Server. El repositorio no incluye un script completo para crear la base ni migraciones de EF Core. Debes disponer del esquema y procedimientos esperados por los repositorios de `Servicios/`; por ejemplo, el registro de usuarios invoca `CrearDatosUsuarioNuevo`.

Desde la raíz, configura una conexión local con autenticación de Windows:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ManejoPresupuesto;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
dotnet restore ManejoPresupuesto.csproj
dotnet build ManejoPresupuesto.csproj
dotnet run --project ManejoPresupuesto.csproj
```

El ejemplo de certificado es solo para desarrollo local. Abre la URL indicada por la terminal; el login está en `/Usuarios/Login`. Las funciones de correo requieren `CONFIGURACIONES_EMAIL:EMAIL`, `HOST`, `PORT` y `PASSWORD`.

## Estado

Proyecto de portafolio con dependencias de base de datos pendientes de empaquetar para una instalación desde cero. No hay una demo pública enlazada ni una suite de pruebas automatizadas incluida.

## Configuración y alcance

Usa una base de desarrollo y credenciales propias. Configura secretos mediante variables de entorno o User Secrets; no los incluyas en commits. La compilación no comprueba la disponibilidad de bases de datos, SMTP o APIs externas.

## Autor

[Eduardo Araneda](https://github.com/eduardoaraneda) · [Portafolio](https://eduardoaraneda.github.io/Portafolio/)
