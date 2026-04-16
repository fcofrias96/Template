using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using Template.Application.Caching;
using Template.Application.Contracts.Caching;
using Template.Application.Contracts.Etag;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Etag;
using Template.Application.Repository;
using Template.Application.Services;
using Template.Application.Services.Base;
using Template.Application.SetupOptions;

namespace Template.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(ApiVersioning.Options);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors(Cors.GetOptions(configuration));
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
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

            #region Dependency Injection

            // services
            services.AddScoped(typeof(BaseServiceDeps<>));
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<ICacheNamespaceVersion, CacheNamespaceVersion>();
            services.AddScoped<ICachePolicyRegistry, CachePolicyRegistry>();
            services.AddScoped<ICacheKeyFactory, CacheKeyFactory>();
            services.AddScoped<ICacheKeyBuilder, CacheKeyBuilder>();
            services.AddScoped<IEtagCacheExecutor, EtagCacheExecutor>();
            services.AddScoped<IEtagHttpResponder, EtagHttpResponder>();
            services.AddScoped<ICacheService, MemoryCacheService>();

            services.AddScoped(typeof(IExportService<>), typeof(ExportService<>));

            #endregion Dependency Injection
        }
    }
}