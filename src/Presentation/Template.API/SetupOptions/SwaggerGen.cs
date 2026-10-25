using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Template.Api.Swagger;

namespace Template.Api.SetupOptions
{
    public static class SwaggerGen
    {
        // Same value as JwtBearerDefaults.AuthenticationScheme; avoids a JWT package dependency in the web building block.
        private const string BearerScheme = "Bearer";

        public static readonly Action<SwaggerGenOptions> Options = options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Template API",
                Version = "v1"
            });

            options.EnableAnnotations();

            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerScheme, document)] = []
            });

            options.SchemaFilter<CustomValidationSchemaFilter>();
        };
    }
}
