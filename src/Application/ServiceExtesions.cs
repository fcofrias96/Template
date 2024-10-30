using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Contracts.Services.V1;
using Template.Application.Repository;
using Template.Application.Services.Base;
using Template.Application.Services.V1;
using Template.Application.SetupOptions;

namespace Template.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
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

            // services
            services.AddScoped(typeof(IAsyncServiceAsync<>), typeof(ServiceBaseAsync<>));
            services.AddScoped(typeof(ICreateServiceAsync<>), typeof(CreateServiceAsync<>));
            services.AddScoped(typeof(IDeleteServiceAsync<>), typeof(DeleteServiceAsync<>));
            services.AddScoped(typeof(IExportServiceAsync<>), typeof(ExportServiceAsync<>));
            services.AddScoped(typeof(IReadServiceAsync<>), typeof(ReadServiceAsync<>));
            services.AddScoped(typeof(IUpdateServiceAsync<>), typeof(UpdateServiceAsync<>));



            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICategoryService, CategoryService>();

            #endregion Dependency Injection
        }
    }
}