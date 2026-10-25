# Instrucciones de Configuración y Ejecución del Proyecto

## Arquitectura: monolito modular

La solución es un **monolito modular**: un único host desplegable (`Template.Api`) que compone módulos de negocio aislados. Cada módulo es dueño de su dominio, su lógica, su `DbContext`, su esquema de base de datos y sus endpoints.

```
src/
├── BuildingBlocks/                         # Código transversal, sin conocimiento de los módulos
│   ├── Template.SharedKernel               # Roles de entidad (IEntity, IAuditable, ISoftDeletable, IHasRowVersion) y EntityBase; sin dependencias
│   ├── Template.BuildingBlocks.Application # BaseService, especificaciones, caché, ETags, exportación, wrappers
│   ├── Template.BuildingBlocks.Infrastructure # BaseRepository, configuración EF, auditoría (esquema "audit")
│   ├── Template.BuildingBlocks.Web         # Helpers HTTP compartidos por los *.Api: registro de controladores, resultados ETag
│   └── Template.BuildingBlocks.Monitoring  # Serilog + OpenTelemetry
├── Modules/
│   ├── Catalog/        # Categorías e ítems              (esquema "catalog")
│   ├── Identity/       # Usuarios, roles, permisos, menú, autenticación JWT (esquema "identity")
│   └── Notifications/  # Envío de correo (SMTP)
│       └── Template.Modules.{Módulo}.{Contracts|Domain|Application|Infrastructure|Api}
└── Presentation/
    ├── Template.API    # Host: compone los módulos y configura MVC, Swagger, versionado, CORS, compresión
    └── Template.Web    # Front-end Angular
tests/
└── Template.ArchitectureTests  # Verifica las reglas de dependencia sobre los ensamblados compilados
```

### Reglas de dependencia

| Proyecto | Puede referenciar |
|---|---|
| `*.Contracts` | nada de la solución (API pública del módulo) |
| `*.Domain` | `SharedKernel`, `Contracts` |
| `*.Application` | su `Domain`, `BuildingBlocks.Application`, `Contracts` de otros módulos |
| `*.Infrastructure` | su `Application`, `BuildingBlocks.Infrastructure` |
| `*.Api` | su `Infrastructure`, `BuildingBlocks.Web`, `Contracts` de otros módulos |
| `Template.Api` (host) | los proyectos `*.Api` de cada módulo y los building blocks |

Un módulo **solo** puede hablar con otro a través de su proyecto `*.Contracts`. Por ejemplo, Catalog protege sus endpoints con `[Permission(PermissionResources.Category, PermissionActions.Read)]` de `Identity.Contracts`, e Identity envía correos mediante `IEmailService` de `Notifications.Contracts`. `Template.ArchitectureTests` hace fallar el build si se rompe alguna de estas reglas:

```bash
dotnet test tests/Template.ArchitectureTests
```

### Agregar un módulo nuevo

1. Crear `src/Modules/<Nombre>/Template.Modules.<Nombre>.{Contracts,Domain,Application,Infrastructure,Api}` siguiendo las referencias de la tabla.
2. Darle su propio `DbContext` (un `DbContext` normal) con `HasDefaultSchema("<nombre>")`.
3. Cada capa registra lo suyo:
   - `*.Application` → `Add<Nombre>Application()`: servicios (`AddServicesImplementingBaseService`) y validadores.
   - `*.Infrastructure` → `Add<Nombre>Infrastructure(configuration)`: `AddModuleDbContext<TContext>(configuration, "<Nombre>", schema)`, repositorios y servicios técnicos. Si el módulo necesita transacciones, define su propio `I<Nombre>UnitOfWork : IUnitOfWork` implementado con `UnitOfWork<TContext>`.
   - `*.Api` → `Add<Nombre>Module(services, configuration)`: llama a las dos anteriores y a `AddModuleControllers`.
4. Llamar a `Add<Nombre>Module` desde `Program.cs` y añadir el nombre del módulo a `ModuleBoundaryTests.Modules`.

`AddModuleDbContext` da a todos los módulos lo mismo: SQL Server (cadena del módulo o `Default`, esquema propio) o base en memoria sembrada al arrancar, todos los interceptores transversales registrados (auditoría, tiempos de BD) y un health check de su base de datos.

### Infraestructura transversal

- **Auditoría:** `AuditSaveChangesInterceptor` se aplica a todos los `DbContext` de módulo (incluido el de Identity, que hereda de `IdentityDbContext`). Sella `CreatedBy/At` y `LastModifiedBy/At` y escribe una fila por entidad modificada en el esquema `audit`. Los valores sensibles se enmascaran por módulo: `services.Configure<AuditOptions>(o => o.Mask<User>(nameof(User.PasswordHash)))`.
- **Tiempos de BD:** `DbTimingInterceptor` (Monitoring) mide cada comando (métrica `db.command.duration`) y registra los lentos (> 200 ms).
- **Health checks:** `/health` (readiness) comprueba la base de datos de cada módulo y la de auditoría; `/health/live` (liveness) solo confirma que el proceso responde.

### Entidades

Cada entidad implementa solo los roles que necesita (`Template.SharedKernel`):

| Rol | Qué aporta |
|---|---|
| `IEntity` | `Id` |
| `IAuditable` | `CreatedBy/At`, `LastModifiedBy/At`, sellados por `AuditSaveChangesInterceptor` (solo lectura en la entidad) |
| `ISoftDeletable` | `IsDeleted` + `MarkDeleted()`, con filtro global de consultas |
| `IHasRowVersion` | `RowVersion` para concurrencia optimista y ETags |

`EntityBase` implementa los cuatro (`IEntityBase`), que es lo que exigen `BaseRepository`/`BaseService`. `EntityBaseConfiguration<T>` configura únicamente las columnas de los roles que la entidad implementa.

### Caché

Todo el caché pasa por un único servicio, `ICacheService` (singleton sobre `IMemoryCache`):

```csharp
var data = await _cache.GetOrCreateAsync(key, TimeSpan.FromHours(1), ct => LoadAsync(ct), region: "Category");
_cache.Remove(key);                 // invalida una entrada
_cache.RemoveRegion("Category");    // invalida todas las entradas de la región
```

`BaseService` ya cachea listas, páginas y lecturas por id (con ETag/`304 Not Modified`) e invalida la región de la entidad y la del ítem en cada escritura. Las claves propias de un módulo se centralizan en una clase estática (por ejemplo `IdentityCacheKeys`), y quien modifica los datos debe eliminar esas claves.

## Configuración de Variables de Entorno

Antes de ejecutar el API, es necesario configurar las variables de entorno. Estas variables son esenciales para conectar con las bases de datos y configurar aspectos de seguridad y servicios de correo electrónico.

### Configuración de Conexiones a Bases de Datos

Todos los módulos comparten **una sola base de datos**: cada uno usa su propio esquema (`catalog`, `identity`, `audit`) y su propia tabla `__EFMigrationsHistory` dentro de ese esquema. Basta con configurar `Default`:

```bash
# Para Windows:
setx ConnectionStrings__Default "tu_cadena_de_conexion_aqui"
```

Si un módulo necesita su propia base de datos (por ejemplo, para extraerlo luego como servicio), añade una cadena con el nombre del módulo y tendrá prioridad sobre `Default`. No hace falta cambiar código:

```bash
setx ConnectionStrings__Catalog  "cadena_solo_para_catalog"
setx ConnectionStrings__Identity "cadena_solo_para_identity"
setx ConnectionStrings__Audit    "cadena_solo_para_auditoria"
```

La misma resolución la usan la aplicación y `dotnet ef` (fábricas de diseño), así que no hay cadenas separadas para migraciones.

### Configuración de JWT para Identity

```bash
setx JwtSettings__Key "your-32-character-long-secret-key"
setx JwtSettings__Issuer "your-issuer"
setx JwtSettings__Audience "your-audience"
setx JwtSettings__EmailConfirmationTokenDurationInHours "24"  # Ejemplo: token válido por 24 horas
setx JwtSettings__PasswordResetTokenDurationInMinutes "120"  # Ejemplo: token válido por 120 minutos
setx JwtSettings__LoginTokenDurationInMinutes "60"  # Ejemplo: token válido por 60 minutos
setx JwtSettings__RefreshTokenDurationInDays "7"  # Ejemplo: token válido por 7 dias
setx JwtSettings__ChangeEmailTokenDurationInMinutes "30"  # Ejemplo: token válido por 30 minutos
```

### Configuración del Servicio de Email

```bash
setx MailSettings__EmailFrom "your-email@example.com"
setx MailSettings__SmtpHost "smtp.example.com"
setx MailSettings__SmtpPort "587"  # Ejemplo: puerto para SMTP
setx MailSettings__SmtpUser "your-smtp-username"
setx MailSettings__SmtpPass "your-smtp-password"
setx MailSettings__DisplayName "Your Display Name"
```

## Migraciones y Actualización de Bases de Datos

Cada módulo mantiene sus migraciones junto a su `DbContext`. El host es el proyecto de inicio (contiene `appsettings.json` y `Microsoft.EntityFrameworkCore.Design`). Ejecuta los comandos desde `src/Presentation/Template.API`, que trae `dotnet-ef` 10 como herramienta local (`dotnet tool restore`).

| Módulo | `DbContext` | Proyecto de migraciones | Carpeta | Esquema |
|---|---|---|---|---|
| Catalog | `CatalogDbContext` | `Modules/Catalog/Template.Modules.Catalog.Infrastructure` | `Migrations` | `catalog` |
| Identity | `IdentityModuleDbContext` | `Modules/Identity/Template.Modules.Identity.Infrastructure` | `Migrations` | `identity` |
| Auditoría | `AuditDbContext` | `BuildingBlocks/Template.BuildingBlocks.Infrastructure` | `Auditing/Migrations` | `audit` |

```bash
cd src/Presentation/Template.API
dotnet tool restore

# Catalog
dotnet ef migrations add <Nombre> --startup-project Template.Api.csproj --project ../../Modules/Catalog/Template.Modules.Catalog.Infrastructure --context CatalogDbContext --output-dir Migrations
dotnet ef database update --startup-project Template.Api.csproj --project ../../Modules/Catalog/Template.Modules.Catalog.Infrastructure --context CatalogDbContext

# Identity
dotnet ef migrations add <Nombre> --startup-project Template.Api.csproj --project ../../Modules/Identity/Template.Modules.Identity.Infrastructure --context IdentityModuleDbContext --output-dir Migrations
dotnet ef database update --startup-project Template.Api.csproj --project ../../Modules/Identity/Template.Modules.Identity.Infrastructure --context IdentityModuleDbContext

# Auditoría
dotnet ef migrations add <Nombre> --startup-project Template.Api.csproj --project ../../BuildingBlocks/Template.BuildingBlocks.Infrastructure --context AuditDbContext --output-dir Auditing/Migrations
dotnet ef database update --startup-project Template.Api.csproj --project ../../BuildingBlocks/Template.BuildingBlocks.Infrastructure --context AuditDbContext
```

> **Bases de datos existentes:** las migraciones `Initial` se regeneraron al mover las tablas a los esquemas `catalog`, `identity` y `audit`. No se pueden aplicar sobre una base creada con las migraciones anteriores (tablas en `dbo`). En desarrollo, elimina las bases y ejecuta `database update`; con datos reales, migra las tablas con `ALTER SCHEMA <esquema> TRANSFER dbo.<tabla>` antes de registrar las nuevas migraciones.

Una vez completados estos pasos, el API estará listo para ser ejecutado.
