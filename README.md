
# Instrucciones de Configuración y Ejecución del Proyecto

## Configuración de Variables de Entorno

Antes de ejecutar el API, es necesario configurar las variables de entorno. Estas variables son esenciales para conectar con las bases de datos y configurar aspectos de seguridad y servicios de correo electrónico.

### Configuración de Conexiones a Bases de Datos

```bash
# Para Windows:
setx ConnectionStrings__TemplateConnection "tu_cadena_de_conexion_aqui"
setx ConnectionStrings__TemplateIdentityConnection "tu_cadena_de_conexion_de_identidad_aqui"
setx ConnectionStrings__TemplateAuditConnection "tu_cadena_de_conexion_de_auditoria_aqui"
```

### Configuración de JWT para Identity

```bash
setx JwtSettings__Key "your-32-character-long-secret-key"
setx JwtSettings__Issuer "your-issuer"
setx JwtSettings__Audience "your-audience"
setx JwtSettings__EmailConfirmationTokenDurationInHours "24"  # Ejemplo: token válido por 24 horas
setx JwtSettings__PasswordResetTokenDurationInMinutes "120"  # Ejemplo: token válido por 120 minutos
setx JwtSettings__LoginTokenDurationInMinutes "60"  # Ejemplo: token válido por 60 minutos
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

Navega al directorio `src` y ejecuta los siguientes comandos en la línea de comando para gestionar las migraciones y actualizar las bases de datos para diferentes contextos.

### Migraciones para ApplicationDbContext

```bash
dotnet ef migrations add Initial --project src\Infrastructure\Template.Intrastructure.Persistence\Template.Persistence.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context ApplicationDbContext
dotnet ef database update --project src\Infrastructure\Template.Intrastructure.Persistence\Template.Persistence.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context ApplicationDbContext
```

### Migraciones para AuditDbContext

```bash
dotnet ef migrations add Initial --project src\Infrastructure\Template.Intrastructure.Persistence\Template.Persistence.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context AuditDbContext
dotnet ef database update --project src\Infrastructure\Template.Intrastructure.Persistence\Template.Persistence.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context AuditDbContext
```

### Migraciones para IdentityContext

```bash
dotnet ef migrations add Initial --project src\Infrastructure\Template.Intrastructure.Identity\Template.Identity.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context IdentityContext
dotnet ef database update --project src\Infrastructure\Template.Intrastructure.Identity\Template.Identity.csproj --startup-project src\Presentation\Template.API\Template.API.csproj --context IdentityContext
```

Una vez completados estos pasos, el API estará listo para ser ejecutado.
