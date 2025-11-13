using Serilog;
using Template.Api.Middlewares;
using Template.Application;
using Template.Application.SetupOptions;
using Template.Email;
using Template.Security;
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
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<ETagMiddleware>();
    app.UseSerilogRequestLogging();
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
}