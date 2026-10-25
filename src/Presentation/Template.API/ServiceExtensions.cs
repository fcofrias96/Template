using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using Template.BuildingBlocks.Application;
using Template.Api.Filters;
using Template.Api.SetupOptions;

namespace Template.Api
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Host-level HTTP services: MVC + validation filter, versioning, Swagger,
        /// CORS, compression, health checks, plus the application building blocks.
        /// Modules register their own controllers and validators.
        /// </summary>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationBuildingBlocks();

            services.AddScoped<ValidationFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddApiVersioning(ApiVersioning.Options);
            services.AddCors(Cors.GetOptions(configuration));
            services.AddEndpointsApiExplorer();
            services.AddHealthChecks();
            services.AddMvc(options => options.SuppressAsyncSuffixInActionNames = false);

            services.AddSwaggerGen(SwaggerGen.Options);
            services.AddVersionedApiExplorer(ApiExplorer.Options);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            return services;
        }
    }
}
