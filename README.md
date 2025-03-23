# Sistema de Gestión de Incidencias

Sistema web para la gestión y seguimiento de incidencias técnicas, desarrollado con ASP.NET MVC 5 y Entity Framework.

## Requisitos Previos

- Visual Studio 2019 o superior
- .NET Framework 4.8
- SQL Server 2014 o superior
- Git (opcional, para clonar el repositorio)

## Instalación

1. Clonar o descargar el repositorio:
   git clone https://github.com/Tiago0516/GestionDeIncidentes


2. Abrir la solución `SistemaIncidencias.sln` en Visual Studio.

3. Restaurar los paquetes NuGet:
   - Click derecho en la solución
   - Seleccionar "Restaurar paquetes NuGet"

4. Configurar la cadena de conexión:
   - Abrir `Web.config`
   - Modificar la cadena de conexión "SistemaIncidenciasDB" según tu entorno

5. Ejecutar las migraciones de la base de datos:
   Update-Database

6. Compilar y ejecutar la aplicación (F5).

## Estructura del Proyecto
GestionDeIncidentes/
├── App_Start/ # Configuración inicial de la aplicación

├── Content/ # Archivos CSS y recursos estáticos

├── Controllers/ # Controladores MVC

├── Models/ # Modelos de dominio y ViewModels

├── Repositories/ # Repositorios para acceso a datos

├── Views/ # Vistas MVC

├── Migrations/ # Migraciones de Entity Framework

└── Scripts/ # Archivos JavaScript



### Componentes Principales

- **IncidenciasController**: Maneja las operaciones CRUD de incidencias
- **IncidenciaRepository**: Implementa el patrón repositorio para incidencias
- **SistemaIncidenciasContext**: Contexto de Entity Framework
- **Modelos**: Incidencia, Usuario, Tecnico, Comentario

## Patrones y Prácticas Implementadas

1. **Patrón Repositorio**
   - Abstracción de la capa de datos
   - Interfaces definidas para cada repositorio
   - Implementación de Unit of Work

2. **Inyección de Dependencias**
   - Uso de Ninject como contenedor IoC
   - Configuración en NinjectWebCommon.cs

3. **Code First**
   - Uso de Entity Framework con enfoque Code First
   - Migraciones para control de cambios en BD

4. **Logging**
   - Implementación de log4net
   - Registro de errores y operaciones importantes

5. **Prácticas de Seguridad**
   - Validación de modelos
   - Protección contra CSRF
   - Sanitización de datos

## Funcionalidades Principales

1. **Gestión de Incidencias**
   - Crear, editar y eliminar incidencias
   - Asignación de técnicos
   - Seguimiento de estado
   - Comentarios y actualizaciones

2. **Dashboard**
   - Visualización de estadísticas
   - Filtros por estado y prioridad
   - Paginación de resultados

3. **Sistema de Notificaciones**
   - Registro de cambios
   - Historial de actualizaciones

## Credenciales de Prueba


## Configuración de Logging

Los logs se almacenan en: [RutaAplicacion]/logs/GestionIncidencias.log
