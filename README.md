# AutoControl — Sistema de Control de Alquiler de Autos

Aplicación desarrollada en **ASP.NET Core MVC 8** con **Entity Framework Core** y **MySQL**. Permite administrar clientes, vehículos, contratos y pagos desde una interfaz moderna, adaptable y completamente en español.

## Funciones incluidas

- CRUD completo de **Clientes**.
- CRUD completo de **Vehículos**.
- CRUD completo de **Contratos**.
- CRUD completo de **Pagos**.
- Panel principal con indicadores de operación.
- Reporte de contratos por cliente, con totales e impresión.
- Búsqueda y filtros por estado.
- Cálculo automático del total del alquiler.
- Control de saldo y pagos confirmados o anulados.
- Datos iniciales para probar la aplicación.

## Reglas de negocio

1. Un vehículo no puede tener contratos que se crucen en el mismo periodo.
2. Un vehículo en mantenimiento no puede ser alquilado.
3. La fecha de devolución debe ser posterior a la fecha de inicio.
4. El precio total se calcula multiplicando los días por la tarifa diaria.
5. La suma de pagos confirmados no puede superar el total del contrato.
6. Los pagos con tarjeta o transferencia requieren un número de referencia.
7. Un contrato con saldo pendiente no puede finalizarse.
8. Un cliente o vehículo relacionado con contratos no puede eliminarse; se puede marcar como inactivo.

## Estructura del proyecto

```text
Controllers/       Controladores MVC y procesos CRUD
Datos/             Contexto de Entity Framework Core
Migrations/        Creación de tablas, relaciones y datos iniciales
Models/            Entidades Cliente, Vehículo, Contrato y Pago
Properties/        Configuración para ejecutar el proyecto
Servicios/         Lógica de contratos y pagos
ViewModels/        Modelos específicos para panel y reportes
Views/             Pantallas Razor en español
wwwroot/            Diseño CSS y comportamiento JavaScript
Program.cs         Configuración principal y dependencias
```

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8 o MariaDB compatible
- Visual Studio Code con la extensión **C# Dev Kit**, o Visual Studio 2022

## Cómo ejecutar en Visual Studio Code

1. Clone y abra el repositorio:

   ```bash
   git clone https://github.com/KevinAlexanderFl/preg10__Alquiler_de_autos.git
   cd preg10__Alquiler_de_autos
   code .
   ```

2. En MySQL Workbench o phpMyAdmin ejecute:

   ```sql
   CREATE DATABASE IF NOT EXISTS alquiler_autos
   CHARACTER SET utf8mb4 COLLATE utf8mb4_spanish_ci;
   ```

3. Abra `appsettings.json` y escriba la contraseña de su usuario MySQL. La configuración inicial usa `root` sin contraseña:

   ```json
   "MySql": "Server=localhost;Port=3306;Database=alquiler_autos;User=root;Password=;"
   ```

4. Abra una terminal dentro del proyecto y ejecute:

   ```bash
   dotnet restore
   dotnet run
   ```

5. Ingrese a `http://localhost:5190`. La migración crea automáticamente las cuatro tablas y carga datos de demostración.

## Base de datos

| Tabla | Relación principal | Proceso disponible |
|---|---|---|
| Clientes | Un cliente tiene muchos contratos | Crear, consultar, editar y eliminar |
| Vehículos | Un vehículo tiene muchos contratos | Crear, consultar, editar y eliminar |
| Contratos | Pertenece a un cliente y un vehículo | Crear, consultar, editar, finalizar, cancelar y eliminar |
| Pagos | Pertenece a un contrato | Crear, consultar, editar y eliminar |

Las llaves foráneas mantienen la integridad de la información. Las cédulas, correos y placas poseen índices únicos.

## Organización y principios SOLID

- Los modelos representan únicamente los datos del negocio.
- Los controladores atienden las solicitudes y preparan las vistas.
- `ServicioContratos` y `ServicioPagos` concentran las reglas del alquiler.
- Las interfaces permiten cambiar o probar las implementaciones mediante inyección de dependencias.
- `ContextoAlquiler` se ocupa exclusivamente del acceso a MySQL.

Esta separación reduce el acoplamiento y permite modificar una parte sin alterar innecesariamente las demás.
