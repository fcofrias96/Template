using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts.Services.V1;
using Template.Application.Services;
using Template.Application.Services.V1;
using Template.Application.SetupOptions;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;

namespace Template.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, ConfigurationManager configuration)
        {

            var keyVaultUri = configuration["KeyVault:VaultUri"];
            if (!string.IsNullOrEmpty(keyVaultUri))
            {
                configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
            }

            services.AddApiVersioning(ApiVersioning.Options);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers().ConfigureApiBehaviorOptions(ApiBehavior.Options);
            services.AddCors(Cors.GetOptions(configuration));
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddMvc(options => options.SuppressAsyncSuffixInActionNames = false);
            services.AddSwaggerGen(SwaggerGen.Options);
            services.AddVersionedApiExplorer(ApiExplorer.Options);

            #region Dependency Injection

            services.AddScoped(typeof(IAsyncService<>), typeof(ServiceBase<>));
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICategoryService, CategoryService>();

            #endregion Dependency Injection
        }
    }
}