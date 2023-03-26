using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.Filter;
using Template.Core.Application.Services;
using Template.Core.Application.SetupOptions;

namespace Template.Core.Application
{
    public static class ServiceExtesions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(Cors.Options);
            services.AddControllers().AddOData(OData.Options);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(SwaggerGen.Options);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                     new QueryStringApiVersionReader("api-version"),
                     new HeaderApiVersionReader("X-Version"),
                     new MediaTypeApiVersionReader("v"));
            });

           services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });



            #region Dependency Injection
            services.AddScoped(typeof(IGenericServiceAsync<>), typeof(GenericServiceAsync<>));
            services.AddScoped<IItemServiceAsync, ItemServiceAsync>();
            services.AddScoped<ICategoryServiceAsync, CategoryServiceAsync>();
            services.AddScoped<IParameterFilter, SortablePropertiesParameterFilter>();
            #endregion Dependency Injection
        }
    }
}