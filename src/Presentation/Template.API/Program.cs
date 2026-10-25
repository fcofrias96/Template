using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Template.Api;
using Template.Api.Middlewares;
using Template.BuildingBlocks.Monitoring;
using Template.BuildingBlocks.Monitoring.Middlewares;
using Template.Modules.Catalog.Api;
using Template.Modules.Identity.Api;
using Template.Modules.Notifications.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Host services and cross-cutting building blocks
builder.Services.AddApiServices(configuration);
builder.AddMonitoringInfrastructure(configuration);

// Modules
builder.Services.AddIdentityModule(configuration);
builder.Services.AddCatalogModule(configuration);
builder.Services.AddNotificationsModule(configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template API V1");
        c.RoutePrefix = "swagger";
        c.IndexStream = () => File.OpenRead("wwwroot/swagger/custom.html");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseStaticFiles();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("CorsPolicy");
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Readiness: every module's database must be reachable. Liveness: the process answers (no dependencies).
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
 
try
{
    Log.Information("Starting API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API startup failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}
