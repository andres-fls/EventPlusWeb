# EventPlus Web

Sistema de gestión de eventos desarrollado en ASP.NET MVC (.NET Framework 4.8) con Azure SQL, desplegado en Azure App Service.

🌐 **Demo en vivo:** [eventplusweb-avfah6hxa8gxc6ba.brazilsouth-01.azurewebsites.net](https://eventplusweb-avfah6hxa8gxc6ba.brazilsouth-01.azurewebsites.net)

## Descripción

EventPlus es una plataforma web para crear, gestionar e inscribir participantes en eventos. Cuenta con autenticación de usuarios por roles, dashboard con gráficas, categorización de eventos, sistema de inscripciones, reportes en PDF, chatbot asistente con inteligencia artificial y modo claro/oscuro.

## Tecnologías

- ASP.NET MVC 5 (.NET Framework 4.8)
- C# / ADO.NET
- Azure App Service + Azure SQL
- Bootstrap 5 + Bootstrap Icons
- Chart.js (gráficas del dashboard)
- FastReport OpenSource (reportes PDF)
- Google Gemini API (chatbot IA)
- BCrypt.Net (hash de contraseñas)
- HTML5 / CSS3 / JavaScript

## Funcionalidades

- Landing page pública de presentación
- Autenticación con roles (Admin / Aprendiz)
- Hash de contraseñas con BCrypt
- CRUD completo de eventos con validación de fechas
- Sistema de categorías
- Inscripción y cancelación en eventos (individual y grupal)
- Dashboard con gráficas (Chart.js) adaptadas a modo claro/oscuro
- Panel de administración con gestión de usuarios
- Chatbot EventBot con IA (Gemini) como widget flotante
- Reportes en PDF con FastReport
- Modo claro / oscuro con persistencia en localStorage
- Diseño responsive (móvil y escritorio)
- Manejo de excepciones con Trace en todos los services
- Protección CSRF con ValidateAntiForgeryToken
- Filtro de autenticación centralizado (AuthFilter)

## Estructura del Proyecto

```
EventPlusWeb1/
├── Controllers/
│   ├── HomeController.cs
│   ├── EventosController.cs
│   ├── UsuariosController.cs
│   ├── CategoriasController.cs
│   ├── InscripcionesController.cs
│   ├── ReportesController.cs
│   └── ChatbotController.cs
├── Filters/
│   └── AuthFilter.cs
├── Models/Entities/
│   ├── Categoria.cs
│   ├── Usuario.cs
│   ├── Evento.cs
│   ├── Inscripcion.cs
│   ├── Aprendiz.cs
│   ├── Ficha.cs
│   ├── Programa.cs
│   └── Grupo.cs
├── Services/
│   ├── DatabaseService.cs
│   ├── CategoriaService.cs
│   ├── UsuarioService.cs
│   ├── EventoService.cs
│   ├── InscripcionService.cs
│   ├── AprendizService.cs
│   ├── FichaService.cs
│   ├── ProgramaService.cs
│   ├── GrupoService.cs
│   └── ChatbotService.cs
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _LandingLayout.cshtml
│   ├── Home/
│   │   ├── Landing.cshtml
│   │   └── Index.cshtml
│   ├── Usuarios/
│   ├── Eventos/
│   ├── Categorias/
│   ├── Inscripciones/
│   ├── Reportes/
│   └── ChatBot/
├── Reports/
│   ├── Eventos.frx
│   └── Inscripciones.frx
└── Content/
    └── Site.css
```

## Base de Datos

Azure SQL con las siguientes tablas:

- `Usuario` — Id, Nombre, Correo, ContrasenaHash, Rol, Estado
- `Categoria` — Id, NombreCategoria
- `Evento` — Id, NombreEvento, Descripcion, Lugar, Fechas, Cupo, Tipo, Estado, CategoriaId, UsuarioId
- `Inscripcion` — Id, AprendizId, EventoId, GrupoId, FechaInscripcion, Estado
- `Aprendiz` — Id, UsuarioId, FichaId, Cedula, Telefono, Edad, Genero
- `Ficha` — Id, ProgramaId, CodigoFicha, FechaInicio, FechaFin, Estado
- `Programa` — Id, CodigoPrograma, NombrePrograma, DuracionMeses, Nivel
- `Grupo` — Id, EventoId, NombreGrupo

## Configuración Local

1. Clonar el repositorio
2. Crear `AppSecrets.config` en la raíz del proyecto:
```xml
<appSettings>
    <add key="GeminiApiKey" value="TU_API_KEY_AQUI"/>
</appSettings>
```
3. Crear `ConnectionStrings.config` en la raíz del proyecto:
```xml
<add name="EventPlusDB"
     connectionString="Server=localhost;Database=EventPlusDB;
     Trusted_Connection=True;"
     providerName="System.Data.SqlClient" />
```
4. Restaurar paquetes NuGet
5. Ejecutar el script SQL en `/Scripts/BaseDeDatos.sql`
6. Ejecutar el proyecto desde Visual Studio

> ⚠️ Los archivos `AppSecrets.config` y `ConnectionStrings.config` están en `.gitignore` — nunca se suben al repositorio.

## Autor

**Andrés Felipe Leal Sánchez**
Tecnología en Análisis y Desarrollo de Software (ADSO)
Centro para la Industria Petroquímica — SENA Regional Bolívar
2025 - 2026

## Estado

✅ Desplegado en producción — En desarrollo activo
