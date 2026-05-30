# EventPlus Web

Sistema de gestión de eventos desarrollado en ASP.NET MVC (.NET Framework 4.8) con SQL Server.

## Descripción

EventPlus es una plataforma web para crear, gestionar e inscribirse en eventos. Incluye autenticación de usuarios, categorización de eventos, sistema de inscripciones, reportes en PDF y un chatbot asistente con inteligencia artificial (Gemini).

## Tecnologías

- ASP.NET MVC 5 (.NET Framework 4.8)
- C#
- SQL Server (ADO.NET)
- Bootstrap 5
- FastReport OpenSource (reportes PDF)
- API de Google Gemini (chatbot IA)
- HTML5 / CSS3 / JavaScript

## Características

- Autenticación de usuarios (Login/Registro)
- CRUD completo de eventos
- Sistema de categorías
- Inscripción y cancelación en eventos
- Panel de administración (rol Admin)
- Chatbot asistente con IA (widget flotante)
- Generador de descripciones con IA
- Reportes en PDF con FastReport
- Diseño responsive modo oscuro

## Base de Datos

SQL Server local con las siguientes tablas:
- Categorias (Id, Nombre)
- Usuarios (Id, Nombre, Correo, Contrasena, Rol)
- Eventos (Id, Titulo, Descripcion, FechaEvento, Ubicacion, CategoriaId, UsuarioCreadorId)
- Inscripciones (Id, UsuarioId, EventoId, FechaInscripcion)

## Configuración

1. Crear la base de datos "EventPlusDB" en SQL Server
2. Ejecutar el script SQL incluido en /Scripts/BaseDeDatos.sql
3. Verificar la cadena de conexión en Services/DatabaseService.cs
4. Restaurar paquetes NuGet
5. Ejecutar el proyecto desde Visual Studio

## Cadena de Conexión

```
Server=localhost;Database=EventPlusDB;Trusted_Connection=True;
```

## Estructura del Proyecto

```
EventPlusWeb1/
├── Controllers/
│   ├── HomeController.cs
│   ├── EventosController.cs
│   ├── UsuariosController.cs
│   ├── CategoriasController.cs
│   └── ChatbotController.cs
├── Models/Entities/
│   ├── Categoria.cs
│   ├── Usuario.cs
│   ├── Evento.cs
│   └── Inscripcion.cs
├── Services/
│   ├── DatabaseService.cs
│   ├── CategoriaService.cs
│   ├── UsuarioService.cs
│   ├── EventoService.cs
│   └── InscripcionService.cs
├── Views/
│   ├── Shared/_Layout.cshtml
│   ├── Usuarios/
│   ├── Eventos/
│   └── Categorias/
└── Content/
```

## Autor

Andrés - Análisis y Desarrollo de Software (ADSO)
SENA 2025

## Estado

En desarrollo
