# \# ManejoPresupuesto

# 

# Proyecto Razor Pages para la gestión de presupuesto.

# 

# \## Descripción

# 

# Aplicación web basada en Razor Pages que proporciona funcionalidades básicas para el manejo de presupuestos personales o de proyectos. Diseñada para .NET 10.

# 

# \## Requisitos

# 

# \- .NET 10 SDK

# \- Visual Studio 2026 (o `dotnet` CLI)

# 

# \## Restaurar dependencias

# 

# Desde la terminal (PowerShell) en la raíz del repositorio:

# 

# ```powershell

# cd "C:\\Users\\albo\_\\OneDrive\\Documentos\\Proyectos C#\\ManejoPresupuesto\\"

# dotnet restore "ManejoPresupuesto\\ManejoPresupuesto.csproj"

# ```

# 

# En Visual Studio: usar `Restore NuGet Packages` desde el menú contextual del proyecto.

# 

# \## Compilar y ejecutar

# 

# Con `dotnet`:

# 

# ```powershell

# dotnet build "ManejoPresupuesto\\ManejoPresupuesto.csproj"

# dotnet run --project "ManejoPresupuesto\\ManejoPresupuesto.csproj"

# ```

# 

# En Visual Studio: presionar `F5` o usar `IIS Express`/`Kestrel` desde la barra de depuración.

# 

# \## Comprobar y corregir vulnerabilidades de paquetes NuGet

# 

# Pasos recomendados para identificar y corregir vulnerabilidades:

# 

# 1\. Restaurar paquetes:

# 

# ```powershell

# dotnet restore

# ```

# 

# 2\. Listar paquetes vulnerables e información de versiones:

# 

# ```powershell

# dotnet list package --vulnerable

# dotnet list package --outdated

# ```

# 

# 3\. Actualizar paquetes afectados:

# 

# \- Usar el administrador de paquetes de Visual Studio: `Manage NuGet Packages` -> `Updates`.

# \- O con `dotnet` CLI, especificando la versión segura conocida:

# 

# ```powershell

# dotnet add ManejoPresupuesto\\ManejoPresupuesto.csproj package <PackageName> --version <NewVersion>

# ```

# 

# 4\. Volver a compilar y ejecutar pruebas después de actualizar paquetes.

# 

# 5\. (Opcional) Usar herramientas externas para auditoría de seguridad:

# 

# \- `dotnet list package --vulnerable` (integrado)

# \- `dotnet outdated` (herramienta comunitaria)

# \- Servicios externos como GitHub Dependabot o Snyk para escaneo continuo.

# 

# \## Publicar paquete NuGet

# 

# 1\. Aumentar la versión en `ManejoPresupuesto.csproj` (o en el archivo de proyecto donde se define):

# 

# ```xml

# <PropertyGroup>

# &#x20; <Version>1.0.0</Version>

# &#x20; <PackageId>ManejoPresupuesto</PackageId>

# &#x20; <Authors>TuNombre</Authors>

# </PropertyGroup>

# ```

# 

# 2\. Empaquetar y publicar:

# 

# ```powershell

# dotnet pack -c Release --no-build

# dotnet nuget push "bin\\Release\\ManejoPresupuesto.\*.nupkg" --api-key <API\_KEY> --source https://api.nuget.org/v3/index.json

# ```

# 

# Nota: asegúrate de que el paquete no incluya dependencias con vulnerabilidades; actualiza antes de publicar.

# 

# \## Contribuir

# 

# \- Abrir un issue para reportar errores o vulnerabilidades.

# \- Crear pull requests con cambios claros y pruebas cuando corresponda.

# 

# \---

# Archivo generado automáticamente. Mantenerlo actualizado con pasos y versiones específicas del proyecto.

