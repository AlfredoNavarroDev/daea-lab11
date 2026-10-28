# Lab11 - Eventos e Inscripciones (EF Core Code First)

Aplicación web ASP.NET Core MVC para gestionar eventos, asistentes e inscripciones. La base de datos (`EventosDB`, SQL Server) se genera completamente con EF Core **Code First**: las entidades en C# definen el esquema, y las migraciones crean/actualizan las tablas. No hay scripts SQL manuales ni procedimientos almacenados; todo el acceso a datos se hace con LINQ.

## De qué trata

- **Categoria**: categorías de eventos (nombre único, baja lógica).
- **Evento**: título, fecha, lugar, aforo, precio, modalidad (Presencial/Virtual), categoría asociada, baja lógica.
- **Asistente**: nombres, DNI (8 dígitos, único), correo (único), teléfono opcional, baja lógica.
- **Inscripcion**: relaciona un Asistente con un Evento; Estado = `Confirmada` o `Cancelada`.

Funcionalidad principal:
- **Eventos**: listado de eventos activos con filtro por categoría, orden por fecha, inscritos confirmados y cupos disponibles calculados; alta, edición, detalle y baja lógica.
- **Asistentes**: listado y alta, validando que DNI y correo no estén duplicados.
- **Inscripciones**: alta validando que el evento no haya pasado, que el asistente no esté ya inscrito (Confirmada) en ese evento y que el aforo no esté completo; listado con opción de cancelar (Estado = Cancelada).

Todas las acciones de guardado usan el patrón **Post/Redirect/Get** y muestran el resultado con `TempData`. Las lecturas usan `AsNoTracking()`; no hay `FromSqlRaw` ni SQL escrito a mano.

## Estructura del proyecto

```
Lab11/
├── Lab11.slnx                     # Solución
└── Lab11/                         # Proyecto ASP.NET Core MVC
    ├── Models/                    # Entidades EF Core (Data Annotations, sin Fluent API)
    │   ├── Categoria.cs
    │   ├── Evento.cs
    │   ├── Asistente.cs
    │   └── Inscripcion.cs
    ├── ViewModels/
    │   └── EventoListItemViewModel.cs   # Proyección para el Index de Eventos (inscritos/cupos)
    ├── Data/
    │   └── EventosContext.cs      # DbContext con un DbSet por entidad (sin OnModelCreating)
    ├── Repositorios/              # Toda la lógica de acceso a datos (LINQ + SaveChangesAsync)
    │   ├── EventoRepositorio.cs
    │   ├── AsistenteRepositorio.cs
    │   └── InscripcionRepositorio.cs
    ├── Controllers/                # No usan EventosContext directamente, solo repositorios
    │   ├── EventosController.cs
    │   ├── AsistentesController.cs
    │   ├── InscripcionesController.cs
    │   └── HomeController.cs
    ├── Views/
    │   ├── Eventos/               # Index, Details, Create, Edit, Delete
    │   ├── Asistentes/            # Index, Create
    │   ├── Inscripciones/         # Index, Create
    │   └── Shared/_Layout.cshtml  # Menú: Eventos, Asistentes, Inscripciones
    ├── Migrations/                 # 3 migraciones (ver abajo), no editadas a mano salvo lo indicado
    ├── Program.cs                  # AddDbContext, AddScoped repos, seed inicial (3 categorías, 4 eventos, 5 asistentes)
    └── appsettings.json            # Cadena de conexión "EventosDB"
```

## Migraciones (3 en total)

| # | Nombre | Qué agrega |
|---|--------|------------|
| 1 | `InitialCreate` | Las 4 entidades y sus relaciones |
| 2 | `AgregarModalidadEvento` | `Evento.Modalidad` (obligatorio, máx. 20, default `Presencial`) |
| 3 | `AgregarTelefonoAsistente` | `Asistente.Telefono` (opcional, máx. 9) |

## Comandos usados

Requiere la herramienta global `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

Compilar el proyecto:

```bash
dotnet build
```

Crear y aplicar las migraciones (en orden, dentro de `Lab11/Lab11`):

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add AgregarModalidadEvento
dotnet ef database update

dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

Ejecutar la aplicación:

```bash
dotnet run
```

## Base de datos

Cadena de conexión en `appsettings.json` (LocalDB por defecto, ajustar `Server` según el entorno):

```json
"ConnectionStrings": {
  "EventosDB": "Server=(localdb)\\mssqllocaldb;Database=EventosDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Evidencia de migraciones aplicadas en SQL Server:

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Debe devolver 3 filas, con `MigrationId` igual al nombre de archivo de cada migración (`aaaaMMddHHmmss_NombreDeLaMigración`).
