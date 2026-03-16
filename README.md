# UserManagementAPI

Backend desarrollado en **ASP.NET Core Web API (.NET 8)** para la gestión de usuarios, estudios y direcciones.

## Requisitos

* .NET 8 SDK
* SQL Server LocalDB
* Visual Studio 2022 (o Visual Studio Code)

## Clonar el repositorio

```bash
git clone https://github.com/music1991/UserManagementAPI.git
cd UserManagementAPI
```

## Ejecutar el proyecto

1. Abrir la solución `UserManagementAPI.sln` en Visual Studio.

2. El proyecto tiene la opcion de realizar las migraciones automaticas al inciar la solucion en Play.

En caso de que quiera hacerlo manual, debe comentar la seccion del codigo correspondiente en Program.cs, es decir el bucle debajo de la region "#region Middleware Pipeline" y ejutar manualmente el comando:

```powershell
Update-Database
```

Esto creará automáticamente:

* La base de datos
* Las tablas
* Un usuario administrador inicial

3. Ejecutar la API:

```powershell
dotnet run
```

o ejecutar el proyecto desde Visual Studio.

## Usuario administrador inicial

```
Email: admin@test.com
Password: hola123
Role: Admin
```

## Swagger

Una vez ejecutado el proyecto, Swagger estará disponible en:

```
https://localhost:{port}/swagger
```

Port esta definido en el file appsetting.Development.json
