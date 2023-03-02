using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Application.Mappings;
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
            services.AddAutoMapper(typeof(AutomapperConfig));
        }
    }
}