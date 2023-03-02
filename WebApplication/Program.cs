using Serilog;
using WebApi.Extensions;
using Template.Core.Application;
using Template.Core.Application.SetupOptions;
using Template.Infrastructure.Identity;
using Template.Infrastructure.Persistence;

{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    #region Add services to the container

    builder.Services.AddApplicationLayer(configuration);
    builder.Services.AddIdentityInfrastructure(configuration);
    builder.Services.AddPersistenceInfrastructure(configuration);
   

    #endregion Add services to the container

    builder.Host.UseSerilog(SeriLog.Options);
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.AddMiddlewares();
    app.MapControllers();
    app.Run();
}