using Serilog;
using SimetricaConsulting.Api.Controllers;
using Template.Api.Middlewares;
using Template.Application;
using Template.Application.SetupOptions;
using Template.Email;
using Template.Identity;
using Template.Persistence;

{
    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;

    #region Add services to the container

    builder.Services.AddApplicationLayer(configuration);
    builder.Services.AddIdentityInfrastructure(configuration);
    builder.Services.AddPersistenceInfrastructure(configuration);
    builder.Services.AddInfrastructureEmail(configuration);

    #endregion Add services to the container

    builder.Host.UseSerilog(SeriLog.Options);
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<ETagMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
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
        Log.CloseAndFlush();
    }
}