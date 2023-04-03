using Serilog;
using Template.Api.Middlewares;
using Template.Core.Application;
using Template.Core.Application.SetupOptions;
using Template.Infrastructure.Email;
using Template.Infrastructure.Identity;
using Template.Infrastructure.Persistence;

{
    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddUserSecrets<Program>();
    }
    else
    {
        builder.Configuration.AddEnvironmentVariables();
    }

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
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    try
    {
        Log.Information("Starting API");
        app.Run();
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