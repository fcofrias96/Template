using Serilog;
using Template.Api.Middlewares;
using Template.Application;
using Template.Email;
using Template.Monitoring;
using Template.Monitoring.Middlewares;
using Template.Persistence;
using Template.Security;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddApplicationLayer(configuration);
builder.Services.AddIdentityInfrastructure(configuration);
builder.Services.AddPersistenceInfrastructure(configuration);
builder.Services.AddInfrastructureEmail(configuration);
builder.AddMonitoringInfrastructure(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseMiddleware<ETagMiddleware>();
app.UseCors("CorsPolicy");
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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