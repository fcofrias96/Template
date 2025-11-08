using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Repository;
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
            services.AddScoped(typeof(IAsyncServiceAsync<,>), typeof(ServiceBaseAsync<,>));
            services.AddScoped(typeof(ICreateServiceAsync<,>), typeof(CreateServiceAsync<,>));
            services.AddScoped(typeof(IDeleteServiceAsync<,>), typeof(DeleteServiceAsync<,>));
            services.AddScoped(typeof(IExportServiceAsync<,>), typeof(ExportServiceAsync<,>));
            services.AddScoped(typeof(IReadServiceAsync<,>), typeof(ReadServiceAsync<,>));
            services.AddScoped(typeof(IUpdateServiceAsync<,>), typeof(UpdateServiceAsync<,>));

            // repositories
            services.AddScoped(typeof(IAsyncRepository<,>), typeof(RepositoryBaseAsync<,>));
            services.AddScoped(typeof(ICreateRepositoryAsync<,>), typeof(CreateRepositoryAsync<,>));
            services.AddScoped(typeof(IDeleteRepositoryAsync<,>), typeof(DeleteRepositoryAsync<,>));
            services.AddScoped(typeof(IReadRepositoryAsync<,>), typeof(ReadRepositoryAsync<,>));
            services.AddScoped(typeof(IUpdateRepositoryAsync<,>), typeof(UpdateRepositoryAsync<,>));

            #endregion Dependency Injection
        }
    }
}